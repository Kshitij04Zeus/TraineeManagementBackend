using TraineeDirectory.Api.DTO;
namespace TrainingDirectory.Api.Services;
public class DirectoryService : IDirectoryService 
{
    public Task<DirectoryTraineeProfileResponse?> GetTraineeProfileAsync(int traineeId)
    {
        if(traineeId<=0) return Task.FromResult<DirectoryTraineeProfileResponse?>(null);;

        var result=new DirectoryTraineeProfileResponse
        {
            TraineeId=traineeId,
            FullName="Kshitij Poojary",
            Email="kshitij.poojary@zeus.com",
            TechStack="Python",
            Status="Active",
            ProfileNote="Internal Directory trainee profile"
        };

        return Task.FromResult<DirectoryTraineeProfileResponse?>(result);
    }
}