using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ISubmissionFileService
{
    Task<SubmissionFileResponse> UploadAsync(int submissionId,int userId,IFormFile file,string correlationId);
    Task<FileDownloadResponse> DownloadAsync(int fileId);
    Task DeleteAsync(int fileId);
    Task<SubmissionFileResponse?> GetMetadataByIdAsync(int fileId,string correlationId);
}