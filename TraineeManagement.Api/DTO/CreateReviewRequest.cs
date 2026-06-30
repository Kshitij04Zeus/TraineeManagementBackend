using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.DTO;

public class CreateReviewRequest
{
    [Required]
    public int SubmissionId {get; set;}

    [Required]
    public int MentorId {get; set;}

    [Required(ErrorMessage = ValidationMessages.FeedbackReq)]
    public string Feedback {get; set;}="";

    public int? Score  {get; set;}=0;
    
    [Required(ErrorMessage = ValidationMessages.ReviewStatusReq)]
    public ReviewStatus ReviewStatus  {get; set;}=ReviewStatus.Accepted;

    [Required(ErrorMessage = ValidationMessages.ReviewedDateReq)]
    public DateTime ReviewedDate  {get; set;}=DateTime.Now;
}