using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SubmissionProcessor.Worker.Constants;
namespace SubmissionProcessor.Worker.Models;

public class SubmissionFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SubmissionId {get; set;}
    public Submission Submission {get; set;}=null!;

    public string OriginalFileName {get; set;}="";
    public string StorageFileName {get; set;}="";
    public string ContentType {get; set;}="";

    public long FileSize {get; set;}
    public string CheckSum {get; set;}="";
    public int UploadedByUserId {get; set;}
    public DateTime CreatedDate {get; set;}=DateTime.Now;
}