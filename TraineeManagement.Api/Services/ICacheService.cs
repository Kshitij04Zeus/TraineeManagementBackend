using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key,T value,TimeSpan? absoluteExpiration = null);
    Task RemoveAsync(string key);
}