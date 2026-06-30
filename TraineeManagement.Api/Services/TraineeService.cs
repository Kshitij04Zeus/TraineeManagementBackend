using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Api.Services;

public class TraineeService : ITraineeService {
  private readonly AppDbContext _context;
  private readonly ILogger<TraineeService> _logger;
  private readonly ICacheService _cacheService;
  public TraineeService(AppDbContext context, ILogger<TraineeService> logger,ICacheService cacheService) 
  {
    _context = context;
    _logger = logger;
    _cacheService=cacheService;
  }

  public async Task<TraineeResponse> AddNewTrainee(CreateTraineeRequest request) 
  {
    var newtrainee = new Trainee 
    {
      FirstName = request.FirstName, LastName = request.LastName,
      Email = request.Email,         TechStack = request.TechStack,
      Status = request.Status,       CreatedDate = DateTime.Now,
      UpdatedDate = DateTime.Now
    };
    await _context.Trainees.AddAsync(newtrainee);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Trainee created successfully with Id: {id}",newtrainee.Id);
    return MapToResponse(newtrainee);
  }

  public async Task<bool> DeleteTrainee(int id) {
    var trainee = await _context.Trainees.FindAsync(id);
    if (trainee == null) 
    {
      throw new KeyNotFoundException($"Trainee not found with Id:{id}");
    }
    _context.Trainees.Remove(trainee);
    await _context.SaveChangesAsync();
    var key=$"trainee:{id}";
    await _cacheService.RemoveAsync(key);
    _logger.LogInformation("Trainee deleted successfully with Id:{id}", id);
    return true;
  }

    public async Task<(List<TraineeResponse>?,int)> GetAllTrainees(int? pageNumber, int? pageSize, string? search, TraineeStatus? status)
    {
      var trainees = await _context.Trainees.ToListAsync();
              
      if (!string.IsNullOrWhiteSpace(search)) {
        search = search.ToLower();
        trainees = trainees.Where(t => t.FirstName.ToLower().Contains(search) ||
                                   t.LastName.ToLower().Contains(search) ||
                                   t.Email.ToLower().Contains(search) ||
                                   t.TechStack.ToLower().Contains(search)).ToList();
      }
      if (status != null) {
        trainees = trainees.Where(t => t.Status == status).ToList();
      }
      var totalRecords = trainees.Count;
      _logger.LogInformation("Total Number of Query Records: {count}",
                             totalRecords);
                  
      var items = trainees.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                      .Take(pageSize ?? 10)
                      .Select(MapToResponse)
                      .ToList();

      if (trainees.Count() == 0 && !string.IsNullOrWhiteSpace(search)) {
        _logger.LogWarning("Trainee not found");
        return (null, 0);
      }

      return (items, totalRecords);
    }

    public async Task<TraineeResponse?> GetById(int id) {
        var key=$"trainee:{id}";
        var cachedTrainee=await _cacheService.GetAsync<TraineeResponse>(key);
        if(cachedTrainee!=null)
        {
          return cachedTrainee;
        }
        var trainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);;
        if (trainee == null) {
          throw new KeyNotFoundException($"Trainee not found with Id {id}");
        }
        var response=MapToResponse(trainee);
        await _cacheService.SetAsync(key,response,TimeSpan.FromMinutes(10));
        return response;
    }

    public async Task<TraineeResponse?> UpdateTrainee(int id, UpdateTraineeRequest request) 
    {
      var updateTrainee = await _context.Trainees.FindAsync(id);
      if (updateTrainee == null)
        throw new KeyNotFoundException($"No Trainee found with the Id {id}");
      updateTrainee.FirstName = request.FirstName;
      updateTrainee.LastName = request.LastName;
      updateTrainee.Email = request.Email;
      updateTrainee.TechStack = request.TechStack;
      updateTrainee.Status = request.Status;
      updateTrainee.UpdatedDate = DateTime.Now;
      await _context.SaveChangesAsync();

      var key=$"trainee:{id}";
      await _cacheService.RemoveAsync(key);

      _logger.LogInformation("Trainee updated successfully with Id:{id}", id);
      return MapToResponse(updateTrainee);
    }

    private static TraineeResponse MapToResponse(Trainee request) {
      return new TraineeResponse { Id = request.Id,
                                   FirstName = request.FirstName,
                                   LastName = request.LastName,
                                   Email = request.Email,
                                   TechStack = request.TechStack,
                                   Status = request.Status,
                                   CreatedDate = request.CreatedDate,
                                   UpdatedDate = request.UpdatedDate };
    }
}
