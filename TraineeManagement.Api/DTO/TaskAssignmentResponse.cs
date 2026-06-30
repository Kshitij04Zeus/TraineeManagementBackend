using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class TaskAssignmentResponse
{
    public int Id {get; set;}

    public int TraineeId {get; set;}
    
    public string TraineeName{get; set;}=null!;

    public int MentorId {get; set;}

    public string MentorName {get; set;} = ""; 

    public int LearningTaskId {get; set;}

    public string LearningTaskTitle {get; set;} = ""; 

    public string LearningTaskDescription {get; set;} = ""; 

    public DateTime AssignedDate {get; set;}=DateTime.Now;

    public DateTime DueDate {get; set;}=DateTime.Now.AddDays(5);

    public TaskAssignmentStatus Status {get; set;}=TaskAssignmentStatus.Assigned;

    public string Remarks {get; set;}="";
}