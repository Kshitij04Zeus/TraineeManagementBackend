using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTO;

public class LearningTaskResponse
{
    public int Id {get; set;}

    public string Title {get; set;}="";

    public string Description {get; set;}="";

    public string ExpectedTechStack {get; set;}="";

    public LearningTaskStatus Status {get; set;}=LearningTaskStatus.Draft;

    public DateTime DueDate {get; set;}=DateTime.Now;

    public DateTime CreatedDate {get; set;}=DateTime.Now;

    public DateTime UpdatedDate {get; set;}=DateTime.Now;
}