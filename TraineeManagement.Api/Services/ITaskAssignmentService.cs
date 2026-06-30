using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ITaskAssignmentService
{
    Task<TaskAssignmentResponse?> GetByIdAssignment(int id);
    Task<TaskAssignmentResponse> CreateAssignment(CreateTaskAssignmentRequest request);
    Task<bool> UpdateStatus(int id, UpdateTaskAssignmentRequest request);
    Task<List<TaskAssignmentResponse>> GetAllAssignments();
}