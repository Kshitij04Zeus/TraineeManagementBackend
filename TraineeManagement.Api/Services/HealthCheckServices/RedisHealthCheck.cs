using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services.HealthCheckServices;
public class RedisHealthCheck:IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    public RedisHealthCheck(IConnectionMultiplexer redis)
    {
        _redis=redis;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken=default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.PingAsync();
            return HealthCheckResult.Healthy("Redis is reachable");
        }
        catch (RedisConnectionException ex)
        {
            return HealthCheckResult.Unhealthy("Redis server is down or unreachable.", ex);
        }
        catch (RedisTimeoutException ex)
        {
            return HealthCheckResult.Unhealthy("Redis responded too slowly (Timeout).", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("An unexpected error occurred during Redis health check.", ex);
        }

    }
}