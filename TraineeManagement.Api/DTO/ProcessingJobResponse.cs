using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class ProcessingJobResponse
{
    public int Id {get; set;}
    public Guid MessageId {get; set;}
    public string CorrelationId {get; set;}=string.Empty;
    public int SubmissionId {get; set;}
    public int FileId {get; set;}
    public ProcessingJobStatus Status {get; set;}
    public int Attempts {get; set;}=0;
    public string? ErrorSummary {get; set;}
    public DateTime StartedAt {get; set;}
    public DateTime CompletedAt {get; set;}
}