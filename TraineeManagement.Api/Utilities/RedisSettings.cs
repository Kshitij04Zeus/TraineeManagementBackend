public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "TraineeManagement:";
    public int DefaultTtlMinutes { get; set; } = 10;
}