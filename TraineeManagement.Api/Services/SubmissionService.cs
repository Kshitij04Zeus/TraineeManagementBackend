using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services;

public class SubmissionService : ISubmissionService {
  private readonly AppDbContext _context;
  private readonly ILogger<SubmissionService> _logger;
  private readonly ICacheService _cacheService;

  public SubmissionService(AppDbContext context,ILogger<SubmissionService> logger,ICacheService cacheService) {
    _context = context;
    _logger = logger;
    _cacheService=cacheService;
  }

  public async Task<SubmissionResponse> CreateSubmission(CreateSubmissionRequest request)
  {
      var taskassignment =await _context.TaskAssignments.FindAsync(request.TaskAssignmentId);
      if (taskassignment == null) 
      {
        throw new KeyNotFoundException("Task Assignment Not Found");
      }

      var newsubmission = new Submission 
      {
          TaskAssignmentId = request.TaskAssignmentId,
          SubmissionUrl = request.SubmissionUrl, Notes = request.Notes ?? "",
          SubmittedDate = request.SubmittedDate, Status = request.Status
      };
      await _context.Submissions.AddAsync(newsubmission);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Submission created successfully with Id: {id}",newsubmission.Id);
      return MapToResponse(newsubmission);
  }

  public async Task<List<SubmissionResponse>> GetAllSubmissions() 
  {
      var submissions = await _context.Submissions.Include(t => t.TaskAssignment).ToListAsync();
      return submissions.Select(MapToResponse).ToList();
  }

  public async Task<SubmissionResponse?> GetByIdSubmission(int id) 
  {
      var key=$"submission-summary:{id}";
      var cachedSubmission=await _cacheService.GetAsync<SubmissionResponse>(key);
      if(cachedSubmission!=null) return cachedSubmission;

      var submission = await _context.Submissions.Include(t => t.TaskAssignment).FirstOrDefaultAsync(t => t.Id == id);
      if (submission == null) 
      {
        _logger.LogWarning("Submission not found with Id:{id}", id);
        throw new KeyNotFoundException($"Submission not found with Id {id}");
      }

      var response=MapToResponse(submission);
      await _cacheService.SetAsync(key,response,TimeSpan.FromMinutes(10));
      return response;
  }

    private static SubmissionResponse MapToResponse(Submission request) {
      return new SubmissionResponse {
        Id = request.Id,
        TaskAssignmentId = request.TaskAssignmentId,
        SubmissionUrl = request.SubmissionUrl,
        Notes = request.Notes,
        SubmittedDate = request.SubmittedDate,
        TaskAssignmentStatus = request.TaskAssignment.Status,
        AssignedDate = request.TaskAssignment.AssignedDate,
        DueDate = request.TaskAssignment.DueDate,
        Status = request.Status,
      };
    }
  }