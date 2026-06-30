using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;

namespace TraineeManagement.Api.Models;
public enum SubmissionStatus
{
    Submitted, Resubmitted
}
public class Submission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    [ForeignKey("TaskAssignment")]
    public int TaskAssignmentId {get; set;}
    public TaskAssignment TaskAssignment {get; set;} = null!; 

    [Required(ErrorMessage = ValidationMessages.SubmissionUrlReq)]
    [Url]
    public string SubmissionUrl {get; set;}= "";

    public string? Notes {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.SubmittedDateReq)]
    public DateTime SubmittedDate {get; set;}=DateTime.Now;

    [Required(ErrorMessage = ValidationMessages.TaskAssignmentStatusReq)]
    public SubmissionStatus Status {get; set;}=SubmissionStatus.Submitted;
}
