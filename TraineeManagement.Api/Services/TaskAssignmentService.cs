using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services;

public class TaskAssignmentService : ITaskAssignmentService {
  private readonly AppDbContext _context;
  private readonly ILogger<TaskAssignmentService> _logger;
  private readonly ICacheService _cacheService;

  public TaskAssignmentService(AppDbContext context,ILogger<TaskAssignmentService> logger,ICacheService cacheService) {
    _context = context;
    _logger = logger;
    _cacheService=cacheService;
  }
  public async Task<TaskAssignmentResponse> CreateAssignment(CreateTaskAssignmentRequest request) {
    var trainee = await _context.Trainees.FindAsync(request.TraineeId);
    if (trainee == null) 
    {
      throw new KeyNotFoundException("Trainee Not Found");
    }
    var mentor = await _context.Mentors.FindAsync(request.MentorId);
    if (mentor == null) throw new KeyNotFoundException("Mentor Not Found");

    var learningtask = await _context.LearningTasks.FindAsync(request.LearningTaskId);
    if (learningtask == null)
      throw new KeyNotFoundException("Learning Task Not Found");

    if (request.DueDate < request.AssignedDate)
      throw new ArgumentException("Due Date cannot be before Assigned Date");

    var newassignment =
        new TaskAssignment { TraineeId = request.TraineeId,
                             MentorId = request.MentorId,
                             LearningTaskId = request.LearningTaskId,
                             AssignedDate = request.AssignedDate,
                             DueDate = request.DueDate,
                             Status = request.Status,
                             Remarks = request.Remarks ?? "" };
    await _context.TaskAssignments.AddAsync(newassignment);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Trainee created successfully with Id: {id}",newassignment.Id);
    return MapToResponse(newassignment);
  }

  public async Task<List<TaskAssignmentResponse>> GetAllAssignments() {
    var assignments = await _context.TaskAssignments.Include(t => t.Trainee)
                          .Include(t => t.Mentor)
                          .Include(t => t.LearningTask)
                          .ToListAsync();

    return assignments.Select(MapToResponse).ToList();
  }

  public async Task<TaskAssignmentResponse?> GetByIdAssignment(int id) {
    var key=$"task-assignment:{id}";
    var cachedTaskAssignment=await _cacheService.GetAsync<TaskAssignmentResponse>(key);
    if(cachedTaskAssignment!=null) return cachedTaskAssignment;

    var assignment = await _context.TaskAssignments.Include(t => t.Trainee)
                         .Include(t => t.Mentor)
                         .Include(t => t.LearningTask)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(t => t.Id == id);
    if (assignment == null) 
    {
      _logger.LogWarning("Assignment not found with Id:{id}", id);
      throw new KeyNotFoundException($"Assignment not found with Id {id}");
    }

    var response=MapToResponse(assignment);
    await _cacheService.SetAsync(key,response,TimeSpan.FromMinutes(10));
    return response;
  }

  public async Task<bool> UpdateStatus(int id,UpdateTaskAssignmentRequest request) {
    var assignment = await _context.TaskAssignments.FindAsync(id);
    if (assignment == null) {
      throw new KeyNotFoundException($"Task Assignment not found with Id {id}");
    }
    assignment.Status = request.Status;
    await _context.SaveChangesAsync();
    var key=$"task-assignment:{id}";
    await _cacheService.RemoveAsync(key);
    _logger.LogInformation("Task Assignment updated successfully with Id:{id}",id);
    return true;
  }

  private static TaskAssignmentResponse MapToResponse(TaskAssignment request) {
    return new TaskAssignmentResponse {
      Id = request.Id,
      TraineeId = request.TraineeId,
      TraineeName = $"{request.Trainee.FirstName} {request.Trainee.LastName}",
      MentorId = request.MentorId,
      MentorName = $"{request.Mentor.FirstName} {request.Mentor.LastName}",
      LearningTaskId = request.LearningTaskId,
      LearningTaskTitle = request.LearningTask.Title,
      LearningTaskDescription = request.LearningTask.Description,
      AssignedDate = request.AssignedDate,
      DueDate = request.DueDate,
      Status = request.Status,
      Remarks = request.Remarks
    };
  }
}