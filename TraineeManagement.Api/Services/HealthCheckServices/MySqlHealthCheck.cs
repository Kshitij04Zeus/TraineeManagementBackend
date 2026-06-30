using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services.HealthCheckServices;
public class MySqlHealthCheck:IHealthCheck
{
    private readonly AppDbContext _context;
    public MySqlHealthCheck(AppDbContext context)
    {
        _context=context;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken=default)
    {
        var canConnect=await _context.Database.CanConnectAsync(cancellationToken);
        if(canConnect)
        {
            return HealthCheckResult.Healthy("My SQL is reachable");
        }
            return HealthCheckResult.Unhealthy("Cannot connect to My SQL");
    }
}