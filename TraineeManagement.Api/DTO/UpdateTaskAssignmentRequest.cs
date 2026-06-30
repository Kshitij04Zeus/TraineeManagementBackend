using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class UpdateTaskAssignmentRequest
{
    [Required(ErrorMessage = ValidationMessages.TaskAssignmentStatusReq)]
    public TaskAssignmentStatus Status {get; set;}=TaskAssignmentStatus.Assigned;
}