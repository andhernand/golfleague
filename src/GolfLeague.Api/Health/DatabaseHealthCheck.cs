using GolfLeague.Application.Database;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;

namespace GolfLeague.Api.Health;

public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory)
    : IHealthCheck
{
    public const string Name = "Database";
    private readonly Serilog.ILogger _logger = Log.ForContext<DatabaseHealthCheck>();

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Verbose("Checking Application Health");
            _ = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Database HealthCheck has Failed");
            return HealthCheckResult.Unhealthy("Database is unhealthy");
        }
    }
}