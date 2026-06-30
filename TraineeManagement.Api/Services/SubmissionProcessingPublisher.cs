using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using TraineeManagement.Api.Contracts;
using System.Text;
using System.Text.Json;
using System.Net.Mime;
using System.Security.Authentication;

namespace TraineeManagement.Api.Services;

public class SubmissionProcessingPublisher : ISubmissionProcessingPublisher
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<SubmissionProcessingPublisher> _logger;

    public SubmissionProcessingPublisher(IRabbitMqConnectionFactory connectionFactory,IOptions<RabbitMqSettings> options,
    ILogger<SubmissionProcessingPublisher> logger)
    {
        _connectionFactory=connectionFactory;
        _settings=options.Value;
        _logger=logger;
    }

    public async Task PublishAsync(SubmissionProcessingRequested message)
    {
        try
        {            
            await using var connection=await _connectionFactory.CreateConnectAsync();
            await using var channel=await connection.CreateChannelAsync();
            var mainQueueArguments = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "submission-processing-dead-letter-exchange" },
                { "x-dead-letter-routing-key", "dead-letter" }
            };
            await channel.QueueDeclareAsync(
                queue:
                _settings.SubmissionProcessingQueueName,
                durable:true,
                exclusive:false,
                autoDelete: false,
                arguments: mainQueueArguments
            );
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties=new BasicProperties
            {
                Persistent=true,
                MessageId=message.MessageId.ToString(),
                CorrelationId=message.CorrelationId,
                ContentType="application/json",
                Type=nameof(SubmissionProcessingRequested)
            };

            await channel.BasicPublishAsync(
                exchange:string.Empty,
                routingKey:_settings.SubmissionProcessingQueueName,
                mandatory:true,
                basicProperties:properties,
                body:body
            );

            _logger.LogInformation("RabbitMQ publish success. Message Id ={MessageId}, CorrelationId={correlationId} SubmissionId={SubmissionId},FileId={FileId}",
            message.MessageId,
            message.CorrelationId,
            message.SubmissionId,
            message.FileId        
            );
        }
        catch (BrokerUnreachableException e)
        {
            _logger.LogCritical($"Connection cannot be made to RabbitMQ {e}");
            return;
        }
    }
}