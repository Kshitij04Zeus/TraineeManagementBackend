using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTO;

public class MentorResponse
{
    public int Id {get; set;}

    public string FirstName {get; set;}="";

    public string LastName {get; set;}="";

    public string Email {get; set;}="";

    public string Expertise {get; set;}="";

    public MentorStatus Status {get; set;}=MentorStatus.Active;

    public DateTime CreatedDate {get; set;}
    public DateTime UpdatedDate {get; set;}
}