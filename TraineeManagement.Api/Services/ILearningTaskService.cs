using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ILearningTaskService
{
    Task<(List<LearningTaskResponse>?,int)> GetAllTasks(int? pageNumber, int? pageSize, string? search, LearningTaskStatus? status);
    Task<LearningTaskResponse?> GetByIdTask(int id);
    Task<LearningTaskResponse> AddNewTask(CreateLearningTaskRequest request);
    Task<LearningTaskResponse?> UpdateTask(int id, UpdateLearningTaskRequest request);
    Task<bool> DeleteTask(int id);
}