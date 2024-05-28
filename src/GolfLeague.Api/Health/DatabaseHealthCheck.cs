using GolfLeague.Application.Database;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GolfLeague.Api.Health;

public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory) : IHealthCheck
{
    public const string Name = "Database";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            // TODO: Log Error
            return HealthCheckResult.Unhealthy("Database is unhealthy", e);
        }
    }
}