using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TraineeManagement.Api.Data;
using Microsoft.Extensions.Options;

namespace TraineeManagement.Api.Services.HealthCheckServices;
public class RabbitMqHealthCheck:IHealthCheck
{
    private readonly RabbitMqSettings _settings;
    public RabbitMqHealthCheck(IOptions<RabbitMqSettings> options)
    {
        _settings=options.Value;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken=default)
    {
        var factory=new ConnectionFactory
        {
            HostName = _settings.Host,  
            Port=_settings.Port,              
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
        await using var connection=await factory.CreateConnectionAsync();
        if(connection.IsOpen) return HealthCheckResult.Healthy("Rabbit MQ is reachable");
        return HealthCheckResult.Unhealthy("RabbitMQ is unreachable.");
    }
}