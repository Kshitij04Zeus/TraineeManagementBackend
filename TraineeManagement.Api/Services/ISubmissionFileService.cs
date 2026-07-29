using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using System.Security.Claims;

namespace TraineeManagement.Api.Services;

public interface ISubmissionFileService
{
    Task<SubmissionFileResponse> UploadAsync(int submissionId,int userId,IFormFile file,string correlationId);
    Task<FileDownloadResponse> DownloadAsync(int fileId,ClaimsPrincipal user);
    Task DeleteAsync(int fileId,ClaimsPrincipal user);
    Task<SubmissionFileResponse?> GetMetadataByIdAsync(int fileId,string correlationId,ClaimsPrincipal user);
}