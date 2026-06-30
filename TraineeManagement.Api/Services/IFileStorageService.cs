using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream stream,string extension);
    Task<Stream> OpenReadAsync(string storageName);
    Task<bool> ExistsAsync(string storageName);
    Task DeleteAsync(string storageName);
}