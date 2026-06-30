using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Utilities;

namespace TraineeManagement.Api.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public FileStorageService(IOptions<FileStorageSettings> options)
    {
        _rootPath=options.Value.RootPath;
        Directory.CreateDirectory(_rootPath);
    }
    public async Task<string> SaveAsync(Stream stream, string extension)
    {
        var storageName=$"{Guid.NewGuid()}{extension}";
        var fullpath=Path.Combine(_rootPath,storageName);
        using var fileStream=File.Create(fullpath);
        await stream.CopyToAsync(fileStream);
        return storageName;
    }
    public Task<Stream> OpenReadAsync(string storageName)
    {
        Stream stream=File.OpenRead(Path.Combine(_rootPath,storageName));
        return Task.FromResult(stream);
    }
    public Task<bool> ExistsAsync(string storageName)
    {
        return Task.FromResult(File.Exists(Path.Combine(_rootPath,storageName)));
    }
    public Task DeleteAsync(string storageName)
    {
        var path=Path.Combine(_rootPath,storageName);
        if(File.Exists(path))
        {
            File.Delete(path);
        }
        return Task.CompletedTask;
    }
}