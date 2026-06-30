using SubmissionProcessor.Worker.Contracts;
namespace SubmissionProcessor.Worker.Services;
public interface ITrainingDirectoryClient
{
    Task<DirectoryTraineeProfileResponse?> GetTraineeProfileAsync(int traineeId,string? CorrelationId,CancellationToken cancellationToken);
}