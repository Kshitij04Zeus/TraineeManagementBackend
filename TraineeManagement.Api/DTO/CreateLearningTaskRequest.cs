using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class CreateLearningTaskRequest
{
    [Required(ErrorMessage = ValidationMessages.TitleReq)]
    public string Title {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.DescriptionReq)]
    public string Description {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.ExpectedTechStackReq)]
    public string ExpectedTechStack {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.LearningTaskStatusReq)]
    public LearningTaskStatus Status {get; set;}=LearningTaskStatus.Draft;

    [Required(ErrorMessage = ValidationMessages.DueDateReq)]
    public DateTime DueDate {get; set;}=DateTime.Now;
}