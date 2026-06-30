using TraineeManagement.Api.Contracts;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;
public interface ISubmissionProcessingPublisher
{
    Task PublishAsync(SubmissionProcessingRequested message);
}