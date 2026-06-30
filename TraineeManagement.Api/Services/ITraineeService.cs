using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ITraineeService
{
    Task<(List<TraineeResponse>?,int)> GetAllTrainees(int? pageNumber, int? pageSize, string? search, TraineeStatus? status);
    Task<TraineeResponse?> GetById(int id);
    Task<TraineeResponse> AddNewTrainee(CreateTraineeRequest request);
    Task<TraineeResponse?> UpdateTrainee(int id, UpdateTraineeRequest request);
    Task<bool> DeleteTrainee(int id);
}