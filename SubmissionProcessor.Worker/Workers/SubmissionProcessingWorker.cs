using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SubmissionProcessor.Worker.Utilities;
using SubmissionProcessor.Worker.Contracts;
using System.Collections.Generic;
using SubmissionProcessor.Worker.Models;
using SubmissionProcessor.Worker.Data;
using SubmissionProcessor.Worker.Services;

namespace SubmissionProcessor.Worker.Workers;

public class SubmissionProcessingWorker : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<SubmissionProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public SubmissionProcessingWorker(IOptions<RabbitMqSettings> options, ILogger<SubmissionProcessingWorker> logger,
    IServiceScopeFactory scopeFactory)
    {
        _settings = options.Value;
        _logger = logger;
        _scopeFactory=scopeFactory;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,  
            Port = _settings.Port,              
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        //DEAD LETTER QUEUE
        string dlxName = _settings.DlxName;
        string dlqName = _settings.DlqName;

        await _channel.ExchangeDeclareAsync(
            exchange: dlxName,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken
        );

        await _channel.QueueDeclareAsync(
            queue: dlqName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await _channel.QueueBindAsync(
            queue: dlqName,
            exchange: dlxName,
            routingKey: "dead-letter",
            cancellationToken: cancellationToken
        );
        var mainQueueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", dlxName },
            { "x-dead-letter-routing-key", "dead-letter" }
        };
        
        await _channel.QueueDeclareAsync(
            queue: _settings.SubmissionProcessingQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArguments,
            cancellationToken: cancellationToken
        );
        
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken
        ); 

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        }
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);
                if (message == null)
                {
                    _logger.LogWarning("Received Invalid or Empty Message Payload");
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken
                    );
                    return;
                }

                //IDEMPOTENCY DONE HERE
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context=scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var existingJob=await _context.ProcessingJobs.FirstOrDefaultAsync(x=>x.CorrelationId==message.CorrelationId);

                    if(existingJob!=null && existingJob.Status==ProcessingJobStatus.Completed)
                    {
                        _logger.LogInformation("Duplicate message by RabbitMQ ignored");
                        await _channel.BasicAckAsync(ea.DeliveryTag,false,stoppingToken);
                        return;
                    }
                    _logger.LogInformation("Received message. MessageId:{MessageId}, CorrelationId:{CorrelationId}, SubmissionId:{SubmissionId}",
                        message.MessageId, message.CorrelationId, message.SubmissionId);
                    
                    if (existingJob == null)
                    {
                        existingJob = new ProcessingJob
                        {
                            MessageId=message.MessageId,
                            CorrelationId=message.CorrelationId,
                            SubmissionId=message.SubmissionId,
                            FileId=message.FileId,
                            Status = ProcessingJobStatus.Processing,
                            Attempts = 0,
                            StartedAt = DateTime.Now
                        };
                        _context.ProcessingJobs.Add(existingJob);
                        await _context.SaveChangesAsync(stoppingToken);
                    }
                    else if(existingJob.Status==ProcessingJobStatus.Queued)
                    {
                        existingJob.Status=ProcessingJobStatus.Processing;
                        existingJob.StartedAt=DateTime.Now;
                        _logger.LogInformation($"Processing of Job {existingJob.Id} for message {message.MessageId} has started.");
                    }

                    //SIMULATING THE PROCESSING
                    try
                    {
                        var metadata=await _context.SubmissionFiles.FindAsync(message.FileId);
                        // throw new Exception("Simulated transient failure for retry testing");
                        if (metadata != null)
                        {
                            _logger.LogInformation("Metadata of the File is: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}", 
                                metadata.Id, metadata.OriginalFileName, metadata.FileSize, metadata.ContentType);

                            _logger.LogInformation("Fetching trainee profile from directory for TraineeId: {TraineeId}", 123);
                            var directoryClient = scope.ServiceProvider.GetRequiredService<ITrainingDirectoryClient>();

                            DirectoryTraineeProfileResponse? traineeProfile = null;
                            try
                            {
                                traineeProfile = await directoryClient.GetTraineeProfileAsync(
                                    123, 
                                    message.CorrelationId, 
                                    stoppingToken
                                );
                            }
                            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex.GetType().Name.Contains("BrokenCircuitException"))
                            {
                                _logger.LogWarning(ex, "Training Directory service is unavailable after resilience retries. Executing fallback policy.");
                                traineeProfile = new DirectoryTraineeProfileResponse
                                {
                                    FullName = "Fallback Profile (Service Offline)",
                                    Email = "offline@system.com",
                                    TechStack = "Unknown",
                                    Status = "Unavailable",
                                    ProfileNote = "Populated via local worker fallback policy because directory was unreachable."
                                };
                            }


                            _logger.LogInformation(
                                    "Successfully retrieved profile for {FullName}. Details - Email: {Email}, TechStack: {TechStack}, Status: {Status}", 
                                    traineeProfile.FullName, 
                                    traineeProfile.Email, 
                                    traineeProfile.TechStack, 
                                    traineeProfile.Status
                                );

                            existingJob.Status=ProcessingJobStatus.Completed;
                            existingJob.CompletedAt = DateTime.Now;
                            existingJob.Attempts++;
                            _logger.LogInformation($"Processing of Job {existingJob.Id} for message {message.MessageId} has completed.");
                            await _context.SaveChangesAsync(stoppingToken);
                        }
                        else
                        {  //Permanent Failure
                            _logger.LogWarning("Metadata file not found for FileId: {FileId}", message.FileId);
                            throw new FileNotFoundException($"Metadata file not found for FileId: {message.FileId}");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Business processing logic failed for MessageId {MessageId}", message.MessageId);
                        existingJob.ErrorSummary = ex.Message;
                        bool isPermanentFailure = ex is FileNotFoundException;
                        bool isRetryExhausted = existingJob.Attempts >= _settings.MaxRetryAttempts;
                        if(isPermanentFailure || isRetryExhausted)
                        {
                            existingJob.Status = ProcessingJobStatus.Failed;
                            _logger.LogError("Processing failed for the Job: {JobId}", existingJob.Id);
                            await _context.SaveChangesAsync();
                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue:false,
                                cancellationToken: stoppingToken
                            );
                        } 
                        else
                        {
                            existingJob.Status = ProcessingJobStatus.Queued;
                            existingJob.Attempts++;
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Retrying the Job: {JobId}, Current Attempt:{Attempts}", existingJob.Id,existingJob.Attempts);
                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                        }
                        return;
                    }                    
                }
                
                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );
                
                _logger.LogInformation("Acknowledged Message {MessageId}", message.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing RabbitMQ message");
                if(_channel!=null)
                {
                    await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _settings.SubmissionProcessingQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
            
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }
 
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken: cancellationToken);
            await _connection.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}
