using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;
public interface ISubmissionService
{
    Task<SubmissionResponse?> GetByIdSubmission(int id);
    Task<SubmissionResponse> CreateSubmission(CreateSubmissionRequest request);
    Task<List<SubmissionResponse>> GetAllSubmissions();
}