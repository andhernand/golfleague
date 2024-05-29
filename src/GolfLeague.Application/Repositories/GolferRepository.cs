using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

using Microsoft.Extensions.Logging;

namespace GolfLeague.Application.Repositories;

public class GolferRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<GolferRepository> logger) : IGolferRepository
{
    public async Task<int> CreateAsync(Golfer golfer, CancellationToken token = default)
    {
        logger.LogInformation("Creating {@Golfer}", golfer);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@FirstName", golfer.FirstName, DbType.String, ParameterDirection.Input, 256);
        parameters.Add("@LastName", golfer.LastName, DbType.String, ParameterDirection.Input, 256);
        parameters.Add("@Email", golfer.Email, DbType.String, ParameterDirection.Input, 256);
        parameters.Add("@JoinDate", golfer.JoinDate, DbType.Date, ParameterDirection.Input);
        parameters.Add("@Handicap", golfer.Handicap, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@GolferId", 0, DbType.Int32, ParameterDirection.Output);

        var commandDefinition = new CommandDefinition(
            "dbo.usp_Golfer_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        await connection.ExecuteScalarAsync(commandDefinition);

        var golferId = parameters.Get<int>("@GolferId");
        return golferId;
    }

    public async Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token)
    {
        logger.LogInformation("Retrieving Golfer by Id: {GolferId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golfer = await connection.QueryFirstOrDefaultAsync<Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetGolferById",
                new { GolferId = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return golfer;
    }

    public async Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token)
    {
        logger.LogInformation("Retrieving All Golfers");

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golfers = await connection.QueryAsync<Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return golfers;
    }

    public async Task<bool> UpdateAsync(Golfer golfer, CancellationToken token)
    {
        logger.LogInformation("Updating {@Golfer}", golfer);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Golfer_Update",
                golfer,
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token)
    {
        logger.LogInformation("Deleting Golfer by Id: {GolferId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@GolferId", id, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        var commandDefinition = new CommandDefinition(
            "dbo.usp_Golfer_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        await connection.ExecuteScalarAsync(commandDefinition);

        var result = parameters.Get<int>("@RowCount") > 0;
        return result;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token)
    {
        logger.LogInformation("Checking for existence of Golfer by Id: {GolferId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM [dbo].[Golfer] WHERE GolferId = @id;",
                new { id },
                commandType: CommandType.Text,
                cancellationToken: token));
    }

    public async Task<Golfer?> ExistsByEmailAsync(string email, CancellationToken token = default)
    {
        logger.LogInformation("Checking for existence of Golfer by Email: {Email}", email);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golfer = await connection.QuerySingleOrDefaultAsync<Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetGolferByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return golfer;
    }
}