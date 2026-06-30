using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class ReviewResponse
{
    public int Id {get; set;}

    public int SubmissionId {get; set;}

    public string SubmissionUrl {get; set;} =""; 

    public DateTime SubmittedDate=DateTime.Now;
    public SubmissionStatus SubmissionStatus=SubmissionStatus.Submitted;
    public int MentorId {get; set;}
    public string MentorName {get; set;} =  "";

    public string Feedback {get; set;}="";

    public int? Score  {get; set;}=0;

    public ReviewStatus ReviewStatus  {get; set;}=ReviewStatus.Accepted;

    public DateTime ReviewedDate  {get; set;}=DateTime.Now;
}