using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}