using TraineeDirectory.Api.DTO;
namespace TrainingDirectory.Api.Services;
public interface IDirectoryService
{
    Task<DirectoryTraineeProfileResponse?> GetTraineeProfileAsync(int traineeId);
}