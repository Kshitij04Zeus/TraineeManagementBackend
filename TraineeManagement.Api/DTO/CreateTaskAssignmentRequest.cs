using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class CreateTaskAssignmentRequest
{
    [Required]
    public int TraineeId {get; set;}

    [Required]
    public int MentorId {get; set;}

    [Required]
    public int LearningTaskId {get; set;}

    [Required(ErrorMessage = ValidationMessages.AssignedDateReq)]
    public DateTime AssignedDate {get; set;}=DateTime.Now;

    [Required(ErrorMessage = ValidationMessages.DueDateReq)]
    public DateTime DueDate {get; set;}=DateTime.Now.AddDays(5);

    [Required(ErrorMessage = ValidationMessages.TaskAssignmentStatusReq)]
    public TaskAssignmentStatus Status {get; set;}=TaskAssignmentStatus.Assigned;

    public string? Remarks {get; set;}
}