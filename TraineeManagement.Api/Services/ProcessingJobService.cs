using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using TraineeManagement.Api.Contracts;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;
public class ProcessingJobService : IProcessingJobService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProcessingJobService> _logger;
    private readonly ISubmissionProcessingPublisher _publisher;

    public ProcessingJobService(AppDbContext context,ILogger<ProcessingJobService> logger,ISubmissionProcessingPublisher publisher)
    {
        _context=context;
        _logger=logger;
        _publisher=publisher;
    }

    public async Task<ProcessingJobResponse?> GetByIdAsync(int id)
    {
        var job=await _context.ProcessingJobs.FindAsync(id);
        if(job==null)
        {
            _logger.LogInformation($"No job found with Id:{id}");
            return null;
        }
        return new ProcessingJobResponse
        {
            Id=job.Id,
            MessageId=job.MessageId,
            CorrelationId=job.CorrelationId,
            SubmissionId=job.SubmissionId,
            FileId=job.FileId,
            Status=job.Status,
            Attempts=job.Attempts,
            ErrorSummary=job.ErrorSummary,
            StartedAt=job.StartedAt,
            CompletedAt=job.CompletedAt
        };
    }

    public async Task<ProcessingJobResponse?> RetryProcessingJob(int id)
    {
        var job=await _context.ProcessingJobs.FindAsync(id);
        if(job==null)
        {
            _logger.LogInformation($"No job found with Id:{id}");
            return null;
        }
        using var transaction = await _context.Database.BeginTransactionAsync();
        var message=new SubmissionProcessingRequested
        {
            MessageId=Guid.NewGuid(),
            CorrelationId=job.CorrelationId,
            SubmissionId=job.SubmissionId,
            FileId=job.FileId,
            RequestedAt=DateTime.Now,
            ContractVersion=1
        };
        try
        {
            job.Status=ProcessingJobStatus.Queued;
            await _context.SaveChangesAsync();
            await _publisher.PublishAsync(message);
            await transaction.CommitAsync();
        }
        catch(Exception e)
        {
            await transaction.RollbackAsync();
            _context.ChangeTracker.Clear();
            throw new Exception($"Connection error encountered from producer side: {e.Message}");
        }
        return new ProcessingJobResponse
        {
            Id=job.Id,
            MessageId=message.MessageId,
            CorrelationId=job.CorrelationId,
            SubmissionId=job.SubmissionId,
            FileId=job.FileId,
            Status=job.Status,
            Attempts=job.Attempts,
            ErrorSummary=job.ErrorSummary,
            StartedAt=job.StartedAt,
            CompletedAt=job.CompletedAt
        };
    }
}