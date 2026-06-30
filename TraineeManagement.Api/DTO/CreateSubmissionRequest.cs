using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class CreateSubmissionRequest
{
    [Required]
    public int TaskAssignmentId {get; set;}

    [Required(ErrorMessage = ValidationMessages.SubmissionUrlReq)]
    [Url]
    public string SubmissionUrl {get; set;}= "";

    public string? Notes {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.SubmittedDateReq)]
    public DateTime SubmittedDate {get; set;}=DateTime.Now;

    [Required(ErrorMessage = ValidationMessages.TaskAssignmentStatusReq)]
    public SubmissionStatus Status {get; set;}=SubmissionStatus.Submitted;
}