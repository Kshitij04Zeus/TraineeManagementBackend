using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class SubmissionResponse
{
    public int Id {get; set;}
    public int TaskAssignmentId {get; set;}

    public string SubmissionUrl {get; set;}= "";

    public string? Notes {get; set;}="";

    public DateTime DueDate {get; set;}=DateTime.Now;

    public DateTime AssignedDate {get; set;} =DateTime.Now;

    public TaskAssignmentStatus TaskAssignmentStatus {get; set;}=TaskAssignmentStatus.Assigned;

    public DateTime SubmittedDate {get; set;}=DateTime.Now;

    public SubmissionStatus Status {get; set;}=SubmissionStatus.Submitted;
}