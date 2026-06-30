using TraineeManagement.Api.Contracts;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;
public interface IProcessingJobService
{
    Task<ProcessingJobResponse?> GetByIdAsync(int id);
    Task<ProcessingJobResponse?> RetryProcessingJob(int id);
}