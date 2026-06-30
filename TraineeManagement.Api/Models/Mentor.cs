using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.Models;

public enum MentorStatus
{
    Active,
    Inactive
}
public class Mentor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    [Required(ErrorMessage = ValidationMessages.FirstNameReq)]
    public string FirstName {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.LastNameReq)]
    public string LastName {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.EmailReq)]
    public string Email {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.ExpertiseReq)]
    public string Expertise {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.MentorStatusReq)]
    public MentorStatus Status {get; set;}=MentorStatus.Active;

    public DateTime CreatedDate {get; set;}=DateTime.Now;

    public DateTime UpdatedDate {get; set;}=DateTime.Now;
}