using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

using Serilog;

namespace GolfLeague.Application.Repositories;

public class GolferRepository(IDbConnectionFactory connectionFactory) : IGolferRepository
{
    private readonly ILogger _logger = Log.ForContext<GolferRepository>();

    public async Task<int> CreateAsync(Golfer golfer, CancellationToken token = default)
    {
        _logger.Information("Creating {@Golfer}", golfer);

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

        _ = await connection.ExecuteScalarAsync(commandDefinition);

        var golferId = parameters.Get<int>("@GolferId");
        return golferId;
    }

    public async Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token = default)
    {
        _logger.Information("Retrieving Golfer by {GolferId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golferDictionary = new Dictionary<int, Golfer>();

        _ = await connection.QueryAsync<Golfer, TournamentDetail?, Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetGolferById",
                new { GolferId = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token),
            (golfer, tournamentDetail) =>
            {
                if (!golferDictionary.TryGetValue(golfer.GolferId, out var golferEntry))
                {
                    golferEntry = golfer;
                    golferDictionary.Add(golfer.GolferId, golferEntry);
                }

                if (tournamentDetail is not null)
                {
                    golferEntry.Tournaments.Add(tournamentDetail);
                }

                return golferEntry;
            },
            splitOn: "TournamentId"
        );

        return golferDictionary.Values.SingleOrDefault();
    }

    public async Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token = default)
    {
        _logger.Information("Retrieving All Golfers");

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golferDictionary = new Dictionary<int, Golfer>();

        _ = await connection.QueryAsync<Golfer, TournamentDetail?, Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token),
            (golfer, tournamentDetail) =>
            {
                if (!golferDictionary.TryGetValue(golfer.GolferId, out var golferEntry))
                {
                    golferEntry = golfer;
                    golferDictionary.Add(golfer.GolferId, golferEntry);
                }

                if (tournamentDetail is not null)
                {
                    golferEntry.Tournaments.Add(tournamentDetail);
                }

                return golferEntry;
            },
            splitOn: "TournamentId"
        );

        return golferDictionary.Values;
    }

    public async Task<bool> UpdateAsync(Golfer golfer, CancellationToken token = default)
    {
        _logger.Information("Updating {@Golfer}", golfer);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Golfer_Update",
                new
                {
                    golfer.GolferId,
                    golfer.FirstName,
                    golfer.LastName,
                    golfer.Email,
                    golfer.JoinDate,
                    golfer.Handicap
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        _logger.Information("Deleting {GolferId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@GolferId", id, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        var commandDefinition = new CommandDefinition(
            "dbo.usp_Golfer_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        _ = await connection.ExecuteScalarAsync(commandDefinition);

        var result = parameters.Get<int>("@RowCount") > 0;
        return result;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
    {
        _logger.Information("Checking for the existence of a Golfer with {GolferId}", id);

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
        _logger.Information("Checking for the existence of a Golfer with {Email}", email);

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