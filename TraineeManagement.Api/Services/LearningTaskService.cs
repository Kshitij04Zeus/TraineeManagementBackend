using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Api.Services;

public class LearningTaskService : ILearningTaskService {
  private readonly AppDbContext _context;
  private readonly ILogger<LearningTaskService> _logger;
  public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger) 
  {
      _context = context;
      _logger = logger;
  }

  public async Task<LearningTaskResponse> AddNewTask(CreateLearningTaskRequest request) {
    var newtask = new LearningTask { 
      Title = request.Title,
      Description = request.Description,
      ExpectedTechStack = request.ExpectedTechStack,
      DueDate = request.DueDate,
      Status = request.Status,
      CreatedDate = DateTime.Now,
      UpdatedDate = DateTime.Now 
    };
    await _context.LearningTasks.AddAsync(newtask);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Learning Task created successfully with Id: {id}",newtask.Id);
    return MapToResponse(newtask);
  }

  public async Task<bool> DeleteTask(int id) {
      var task = await _context.LearningTasks.FindAsync(id);
      if (task == null) 
      {
          _logger.LogWarning("Learning Task not found with Id:{id}", id);
          throw new KeyNotFoundException($"Learning Task with ID {id} Not Found");
      }
      _context.LearningTasks.Remove(task);
      await _context.SaveChangesAsync();
      _logger.LogInformation($"Learning Task deleted successfully with Id:{id}");
      return true;
  }

    public async Task<(List<LearningTaskResponse>?,int)> GetAllTasks(int? pageNumber, int? pageSize, string? search, LearningTaskStatus? status)
    {
      var tasks = await _context.LearningTasks.ToListAsync();
      if (!string.IsNullOrWhiteSpace(search)) 
      {
        search = search.ToLower();
        tasks = tasks.Where(t => t.Title.ToLower().Contains(search) ||
                            t.Description.ToLower().Contains(search) ||
                            t.ExpectedTechStack.ToLower().Contains(search)).ToList();
      }
      if (status != null) 
      {
          tasks = tasks.Where(t => t.Status == status).ToList();
      }
      var totalRecords = tasks.Count;
      _logger.LogInformation("Total Number of Query Records: {count}",totalRecords);
      var items = tasks.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                      .Take(pageSize ?? 10)
                      .Select(MapToResponse)
                      .ToList();

      if (tasks.Count() == 0 && !string.IsNullOrWhiteSpace(search)) 
      {
          _logger.LogWarning("Learning Task not found");
          return (null, 0);
      }

      return (items, totalRecords);
    }

    public async Task<LearningTaskResponse?> GetByIdTask(int id) {
      var task = await _context.LearningTasks.FindAsync(id);
      if (task == null) {
        _logger.LogWarning("Learning Task not found with Id:{id}", id);
        throw new KeyNotFoundException($"Learning Task with ID {id} Not Found");
      }
      return MapToResponse(task);
    }

    public async Task<LearningTaskResponse?> UpdateTask(
        int id, UpdateLearningTaskRequest request) {
      var updatedTask = await _context.LearningTasks.FindAsync(id);
      if (updatedTask == null) {
        throw new KeyNotFoundException($"Learning Task with ID {id} Not Found");
        return null;
      }

      updatedTask.Title = request.Title;
      updatedTask.Description = request.Description;
      updatedTask.ExpectedTechStack = request.ExpectedTechStack;
      updatedTask.DueDate = request.DueDate;
      updatedTask.Status = request.Status;
      updatedTask.UpdatedDate = DateTime.Now;
      await _context.SaveChangesAsync();
      _logger.LogInformation("Learning Task updated successfully with Id:{id}", id);
      return MapToResponse(updatedTask);
    }

    private static LearningTaskResponse MapToResponse(LearningTask request) {
      return new LearningTaskResponse { 
      Id = request.Id,
      Title = request.Title,
      Description = request.Description,
      ExpectedTechStack = request.ExpectedTechStack,
      Status = request.Status,
      DueDate = request.DueDate,
      CreatedDate = request.CreatedDate,
      UpdatedDate = request.UpdatedDate
   };
    }
}
