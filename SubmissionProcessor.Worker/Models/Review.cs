using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SubmissionProcessor.Worker.Constants;

namespace SubmissionProcessor.Worker.Models;
public enum ReviewStatus
{
    Accepted, ChangesRequired, Rejected
}
public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    [ForeignKey("Submission")]
    public int SubmissionId {get; set;}
    public Submission Submission {get; set;} = null!; 

    [ForeignKey("Mentor")]
    public int MentorId {get; set;}
    public Mentor Mentor {get; set;} =  null!;

    [Required(ErrorMessage = ValidationMessages.FeedbackReq)]
    public string Feedback {get; set;}="";

    public int? Score  {get; set;}=0;
    
    [Required(ErrorMessage = ValidationMessages.ReviewStatusReq)]
    public ReviewStatus ReviewStatus  {get; set;}=ReviewStatus.Accepted;

    [Required(ErrorMessage = ValidationMessages.ReviewedDateReq)]
    public DateTime ReviewedDate  {get; set;}=DateTime.Now;
}
