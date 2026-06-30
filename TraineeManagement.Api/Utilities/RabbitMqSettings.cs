
public class RabbitMqSettings
{
    public string Host {get; set;} = string.Empty;
    public int Port {get; set;}=0;
    public string VirtualHost { get; set; } = "/";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SubmissionProcessingQueueName {get; set;} = "submission-processing";
    public int MaxRetryAttempts {get; set;}= 3;
    public string DlxName {get; set;}= "submission-processing-dead-letter-exchange";
    public string DlqName {get; set;}= "submission-processing-failed";
}