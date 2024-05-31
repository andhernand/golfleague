using System.Data;

using Microsoft.Data.SqlClient;

using Serilog;

namespace GolfLeague.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class SqlServerDbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly ILogger _logger = Log.ForContext<SqlServerDbConnectionFactory>();

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        _logger.Information("Creating DD Connection");

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }
}