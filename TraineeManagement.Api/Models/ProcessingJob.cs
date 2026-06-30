using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models;
public enum ProcessingJobStatus
{
    Queued, Processing, Completed, Failed
}
public class ProcessingJob
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid MessageId {get; set;}
    public string CorrelationId {get; set;} = string.Empty;
    public int SubmissionId {get; set;}
    public int FileId {get; set;}
    public ProcessingJobStatus Status {get; set;}
    public int Attempts {get; set;}=0;
    public string? ErrorSummary {get; set;}
    public DateTime StartedAt {get; set;}
    public DateTime CompletedAt {get; set;}
}
