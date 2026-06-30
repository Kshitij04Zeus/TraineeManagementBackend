using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubmissionProcessor.Worker.Contracts;

public class SubmissionProcessingRequested
{
    public Guid MessageId {get; set;}
    public string CorrelationId {get; set;} = string.Empty;
    public int SubmissionId {get; set;}
    public int FileId {get; set;}
    public DateTime RequestedAt {get; set;}=DateTime.Now;
    public int ContractVersion {get; set;}=1;
}