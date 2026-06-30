using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.Models;

public enum LearningTaskStatus
{
    Draft,
    Published,
    Closed
}
public class LearningTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

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

    public DateTime CreatedDate {get; set;}=DateTime.Now;

    public DateTime UpdatedDate {get; set;}=DateTime.Now;
}