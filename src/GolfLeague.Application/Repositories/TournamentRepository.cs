using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

using Microsoft.Extensions.Logging;

namespace GolfLeague.Application.Repositories;

public class TournamentRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<TournamentRepository> logger) : ITournamentRepository
{
    public async Task<int> CreateAsync(Tournament tournament, CancellationToken token = default)
    {
        logger.LogInformation("Creating Tournament: {@Tournament}", tournament);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@Name", tournament.Name, DbType.String, ParameterDirection.Input, 256);
        parameters.Add("@Format", tournament.Format, DbType.String, ParameterDirection.Input, 256);
        parameters.Add("@TournamentId", 0, DbType.Int32, ParameterDirection.Output);

        _ = await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_Tournament_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token));

        var tournamentId = parameters.Get<int>("@TournamentId");
        return tournamentId;
    }

    public async Task<Tournament?> GetTournamentByIdAsync(int id, CancellationToken token = default)
    {
        logger.LogInformation("Retrieving Tournament by Id: {TournamentId}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var tournamentDictionary = new Dictionary<int, Tournament>();

        _ = await connection.QueryAsync<Tournament, ParticipationDetail?, Tournament>(
            new CommandDefinition(
                "dbo.usp_Tournament_GetTournamentById",
                new { TournamentId = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token),
            (tournament, detail) =>
            {
                if (!tournamentDictionary.TryGetValue(tournament.TournamentId, out var tournamentEntry))
                {
                    tournamentEntry = tournament;
                    tournamentDictionary.Add(tournament.TournamentId, tournamentEntry);
                }

                if (detail is not null)
                {
                    tournamentEntry.Participants.Add(detail);
                }

                return tournamentEntry;
            },
            splitOn: "GolferId"
        );

        return tournamentDictionary.Values.SingleOrDefault();
    }

    public async Task<Tournament?> GetByNameAndFormat(string name, string format, CancellationToken token = default)
    {
        logger.LogInformation("Retrieving Tournament by {Name} and {Format}", name, format);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var tournament = await connection.QuerySingleOrDefaultAsync<Tournament>(
            new CommandDefinition(
                "dbo.usp_Tournament_GetTournamentByNameAndFormat",
                new { Name = name, Format = format },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return tournament;
    }

    public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default)
    {
        logger.LogInformation("Retrieving all Tournaments");

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var tournamentDictionary = new Dictionary<int, Tournament>();

        _ = await connection.QueryAsync<Tournament, ParticipationDetail?, Tournament>(
            new CommandDefinition(
                "dbo.usp_Tournament_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token),
            (tournament, detail) =>
            {
                if (!tournamentDictionary.TryGetValue(tournament.TournamentId, out var tournamentEntry))
                {
                    tournamentEntry = tournament;
                    tournamentDictionary.Add(tournament.TournamentId, tournamentEntry);
                }

                if (detail is not null)
                {
                    tournamentEntry.Participants.Add(detail);
                }

                return tournamentEntry;
            },
            splitOn: "GolferId"
        );

        return tournamentDictionary.Values;
    }

    public async Task<bool> UpdateAsync(Tournament tournament, CancellationToken token = default)
    {
        logger.LogInformation("Updating {@Tournament}", tournament);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Tournament_Update",
                new { tournament.TournamentId, tournament.Name, tournament.Format },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(int tournamentId, CancellationToken token = default)
    {
        logger.LogInformation("Deleting Tournament by Id: {TournamentId}", tournamentId);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@TournamentId", tournamentId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        _ = await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_Tournament_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token));

        var result = parameters.Get<int>("@RowCount") > 0;
        return result;
    }

    public async Task<bool> ExistsByIdAsync(int tournamentId, CancellationToken token = default)
    {
        logger.LogInformation("Checking for existence of Tournament by Id: {TournamentId}", tournamentId);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM [dbo].[Tournament] WHERE TournamentId = @tournamentId;",
                new { tournamentId },
                commandType: CommandType.Text,
                cancellationToken: token));
    }
}