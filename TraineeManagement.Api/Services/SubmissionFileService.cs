using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using System.Security.Cryptography;
using TraineeManagement.Api.Utilities;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.CustomExceptions;
using TraineeManagement.Api.Contracts;
using RabbitMQ.Client;

namespace TraineeManagement.Api.Services;

public class SubmissionFileService:ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly FileStorageSettings _settings;
    private readonly ILogger _logger;
    private readonly ISubmissionProcessingPublisher _publisher;

    public SubmissionFileService(AppDbContext context,IFileStorageService storage,IOptions<FileStorageSettings> options,ILogger<SubmissionFileService> logger,
    ISubmissionProcessingPublisher publisher)
    {
        _context=context;
        _storage=storage;
        _settings=options.Value;
        _logger=logger;
        _publisher=publisher;
    }

    public async Task<SubmissionFileResponse> UploadAsync(int submissionId,int userId,IFormFile file,string correlationId)
    {
        if(file==null || file.Length==0) throw new ArgumentException("File Is Empty");
        if(file.Length>_settings.MaxFileSizeBytes)
        {
            var maxSizeMb = _settings.MaxFileSizeBytes / (1024 * 1024);
            throw new PayloadTooLargeException($"File size greater than {maxSizeMb} MB.");
        } 
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if(!_settings.AllowedExtensions.Contains(extension)) throw new ArgumentException("File Type Is Not Allowed");
        Console.WriteLine(file.ContentType);
        
        if(!_settings.AllowedContentTypes.Contains(file.ContentType)) throw new ArgumentException("File Content Type Is Not Allowed");

        var submission=await _context.Submissions.FindAsync(submissionId);
        if(submission==null) throw new KeyNotFoundException("Submission Not Found");

        var trainee=await _context.Trainees.FindAsync(userId);
        if(trainee==null) throw new KeyNotFoundException("Trainee Not Found");
        
        string checksum;
        using(var sha256=SHA256.Create())
        {
            using var checksumStream=file.OpenReadStream();
            checksum=Convert.ToHexString(await sha256.ComputeHashAsync(checksumStream));
        }

        var storageName=await _storage.SaveAsync(file.OpenReadStream(),extension);

        var metadata=new SubmissionFile
        {
            SubmissionId=submissionId,
            OriginalFileName=file.FileName,
            StorageFileName=storageName,
            ContentType=file.ContentType,
            FileSize=file.Length,
            CheckSum=checksum,
            CreatedDate=DateTime.Now
        };
        await _context.SubmissionFiles.AddAsync(metadata);
        await _context.SaveChangesAsync();
        var savedMetadata=await _context.SubmissionFiles.FindAsync(metadata.Id) ?? throw new Exception($"Error while saving metadata for ile {metadata.Id}");

        _logger.LogInformation("File Uploaded successfully with the following metadata: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}, Checksum: {Checksum}, CreatedDate: {CreatedDate}", metadata.Id, metadata.OriginalFileName, metadata.FileSize, metadata.ContentType, metadata.CheckSum, metadata.CreatedDate);

        var message=new SubmissionProcessingRequested
        {
            MessageId=Guid.NewGuid(),
            CorrelationId=correlationId,
            SubmissionId=submissionId,
            FileId=metadata.Id,
            RequestedAt=DateTime.Now,
            ContractVersion=1
        };
        try
        {
            await _publisher.PublishAsync(message);
        }
        catch (Exception ex)
        {
            _context.SubmissionFiles.Remove(metadata);
            await _storage.DeleteAsync(metadata.StorageFileName); 
            await _context.SaveChangesAsync();
            throw new Exception($"Connection error encountered from producer side: {ex.Message}"); 
        }

        
        var job=new ProcessingJob
        {
            MessageId=message.MessageId,
            CorrelationId=correlationId,
            SubmissionId=submissionId,
            FileId=metadata.Id,
            Status=ProcessingJobStatus.Queued,
            Attempts=0
        };
        await _context.ProcessingJobs.AddAsync(job);
        await _context.SaveChangesAsync();
        _logger.LogInformation(
    "Processing Job created with the following information: Job Id:{JobId} MessageId:{MessageId} CorrelationId:{CorrelationId} SubmissionId:{SubmissionId} FileId:{FileId} Status:{Status} Attempts:{Attempts}",
    job.Id, job.MessageId, job.CorrelationId, job.SubmissionId, job.FileId, job.Status, job.Attempts);

        return new SubmissionFileResponse
        {
            Id=metadata.Id,
            OriginalFileName=metadata.OriginalFileName,
            ContentType=metadata.ContentType,
            FileSize=metadata.FileSize,
            CreatedDate=metadata.CreatedDate,
            CorrelationId=correlationId
        };
    }

    public async Task<FileDownloadResponse> DownloadAsync(int fileId)
    {
        var metadata=await _context.SubmissionFiles.FindAsync(fileId);
        if(metadata==null) throw new KeyNotFoundException("File Not Found");

        var exists=await _storage.ExistsAsync(metadata.StorageFileName);
        if(!exists) throw new KeyNotFoundException("Physical File missing");

        var stream=await _storage.OpenReadAsync(metadata.StorageFileName);
        _logger.LogInformation("File Downloaded successfully");
        return new FileDownloadResponse
        {
            Stream=stream,
            ContentType=metadata.ContentType,
            FileName=metadata.OriginalFileName
        };
    }

    public async Task DeleteAsync(int fileId)
    {
        var metadata=await _context.SubmissionFiles.FindAsync(fileId);
        if(metadata==null) throw new KeyNotFoundException("File Not Found");
        await _storage.DeleteAsync(metadata.StorageFileName);
        _context.SubmissionFiles.Remove(metadata);
        await _context.SaveChangesAsync();
        _logger.LogInformation("File Deleted successfully with Id: {id}",fileId);
    }

    public async Task<SubmissionFileResponse?> GetMetadataByIdAsync(int fileId,string correlationId)
    {
        var metadata=await _context.SubmissionFiles.FindAsync(fileId);
        if(metadata==null) throw new KeyNotFoundException("File Not Found");
        return new SubmissionFileResponse
        {
            Id=metadata.Id,
            OriginalFileName=metadata.OriginalFileName,
            ContentType=metadata.ContentType,
            FileSize=metadata.FileSize,
            CreatedDate=metadata.CreatedDate,
            CorrelationId=correlationId
        };
        _logger.LogInformation("File Metadata retrieved successfully with Id: {id}",fileId);
    }
}