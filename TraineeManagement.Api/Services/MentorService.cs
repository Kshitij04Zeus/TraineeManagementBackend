using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;

public class MentorService : IMentorService {
  private readonly AppDbContext _context;
  private readonly ILogger<MentorService> _logger;

  public MentorService(AppDbContext context, ILogger<MentorService> logger) 
  {
      _context = context;
      _logger = logger;
  }

  public async Task<MentorResponse?> GetByIdMentor(int id) 
  {
      var mentor = await _context.Mentors.FindAsync(id);
      if (mentor == null) {
        _logger.LogWarning("Mentor not found with Id:{id}", id);
        throw new KeyNotFoundException($"Mentor with ID {id} Not Found");
      }
      return MapToResponse(mentor);
  }

    public async Task<(List<MentorResponse>?,int)> GetAllMentors(int? pageNumber, int? pageSize, string? search, MentorStatus? status)
    {
        var mentors = await _context.Mentors.ToListAsync();
        if (!string.IsNullOrWhiteSpace(search)) 
        {
          search = search.ToLower();
          mentors = mentors.Where(m => m.FirstName.ToLower().Contains(search) ||
                                  m.LastName.ToLower().Contains(search) ||
                                  m.Email.ToLower().Contains(search) ||
                                  m.Expertise.ToLower().Contains(search))
                                  .ToList();
        }
        if (status != null) 
        {
            mentors = mentors.Where(m => m.Status == status).ToList();
        }

        var totalRecords = mentors.Count;
        _logger.LogInformation("Total Number of Query Records: {count}",totalRecords);
        var items = mentors.Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                        .Take(pageSize ?? 10)
                        .Select(MapToResponse)
                        .ToList();
        if (mentors.Count() == 0 && !string.IsNullOrWhiteSpace(search)) 
        {
            _logger.LogWarning("Mentor not found");
            return (null, 0);
        }
        return (items, totalRecords);
    }

    public async Task<MentorResponse> AddNewMentor(CreateMentorRequest request) 
    {
        var newmentor = new Mentor {
            FirstName = request.FirstName, 
            LastName = request.LastName,
            Email = request.Email,         
            Expertise = request.Expertise,
            Status = request.Status,       
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
        await _context.Mentors.AddAsync(newmentor);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor created successfully with Id: {id}",newmentor.Id);
        return MapToResponse(newmentor);
    }

    public async Task<bool> DeleteMentor(int id) {
      var mentor = await _context.Mentors.FindAsync(id);
      if (mentor == null) 
      {
          _logger.LogWarning("Mentor not found with Id:{id}", id);
          throw new KeyNotFoundException($"Mentor with ID {id} Not Found");
      }
      _context.Mentors.Remove(mentor);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Mentor deleted successfully with Id:{id}", id);
      return true;
    }

    public async Task<MentorResponse?> UpdateMentor(int id, UpdateMentorRequest request) {
        var updateMentor = await _context.Mentors.FindAsync(id);
        if (updateMentor == null)
          throw new KeyNotFoundException($"Mentor with ID {id} Not Found");
          
        updateMentor.FirstName = request.FirstName;
        updateMentor.LastName = request.LastName;
        updateMentor.Email = request.Email;
        updateMentor.Expertise = request.Expertise;
        updateMentor.Status = request.Status;
        updateMentor.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor updated successfully with Id:{id}", id);
        return MapToResponse(updateMentor);
    }

    private static MentorResponse MapToResponse(Mentor request) {
      return new MentorResponse { Id = request.Id,
                                  FirstName = request.FirstName,
                                  LastName = request.LastName,
                                  Email = request.Email,
                                  Expertise = request.Expertise,
                                  Status = request.Status,
                                  CreatedDate = request.CreatedDate,
                                  UpdatedDate = request.UpdatedDate };
    }
}