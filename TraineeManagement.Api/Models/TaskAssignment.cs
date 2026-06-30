using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;

namespace TraineeManagement.Api.Models;
public enum TaskAssignmentStatus
{
    Assigned, InProgress, Submitted, Reviewed, Completed 
}
public class TaskAssignment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    [ForeignKey("Trainee")]
    public int TraineeId {get; set;}
    public Trainee Trainee {get; set;} = null!; 

    [ForeignKey("Mentor")]
    public int MentorId {get; set;}
    public Mentor Mentor {get; set;} = null!; 

    [ForeignKey("LearningTask")]
    public int LearningTaskId {get; set;}
    public LearningTask LearningTask {get; set;} = null!; 

    [Required(ErrorMessage = ValidationMessages.AssignedDateReq)]
    public DateTime AssignedDate {get; set;}=DateTime.Now;

    [Required(ErrorMessage = ValidationMessages.DueDateReq)]
    public DateTime DueDate {get; set;}=DateTime.Now.AddDays(5);

    [Required(ErrorMessage = ValidationMessages.TaskAssignmentStatusReq)]
    public TaskAssignmentStatus Status {get; set;}=TaskAssignmentStatus.Assigned;

    public string Remarks {get; set;}="";
}
