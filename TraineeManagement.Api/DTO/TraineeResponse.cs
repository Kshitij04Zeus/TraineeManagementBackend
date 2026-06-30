using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTO;

public class TraineeResponse
{
    public int Id {get; set;}

    public string FirstName {get; set;}="";

    public string LastName {get; set;}="";

    public string Email {get; set;}="";

    public string TechStack {get; set;}="";

    public TraineeStatus Status {get; set;}
    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
}