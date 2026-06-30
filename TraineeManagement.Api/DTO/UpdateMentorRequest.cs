namespace TraineeManagement.Api.DTO;
using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;

public class UpdateMentorRequest
{
    [Required(ErrorMessage = ValidationMessages.FirstNameReq)]
    public string FirstName { get; set; }="";

    [Required(ErrorMessage = ValidationMessages.LastNameReq)]
    public string LastName { get; set; }="";

    [Required(ErrorMessage = ValidationMessages.EmailReq)]
    [EmailAddress]
    public string Email { get; set; }="";

    [Required(ErrorMessage = ValidationMessages.ExpertiseReq)]
    public string Expertise {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.MentorStatusReq)]
    public MentorStatus Status {get; set;}=MentorStatus.Active;
}
