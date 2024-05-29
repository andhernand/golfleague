using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

using Microsoft.Extensions.Logging;

namespace GolfLeague.Application.Repositories;

public class TournamentParticipationRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<TournamentParticipationRepository> logger) : ITournamentParticipationRepository
{
    public async Task<bool> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default)
    {
        logger.LogInformation("Creating {@TournamentParticipation}", tournamentParticipation);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@GolferId", tournamentParticipation.GolferId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@TournamentId", tournamentParticipation.TournamentId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Year", tournamentParticipation.Year.ToString(), DbType.String, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_TournamentParticipation_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token));

        var rowCount = parameters.Get<int>("@RowCount");
        return rowCount > 0;
    }

    public async Task<TournamentParticipation?> GetTournamentParticipationById(
        TournamentParticipation id,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving TournamentParticipation by Id: {@TournamentParticipation}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var tournamentParticipation = await connection.QuerySingleOrDefaultAsync<TournamentParticipation>(
            new CommandDefinition(
                "dbo.usp_TournamentParticipation_GetById",
                id,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return tournamentParticipation;
    }

    public async Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting TournamentParticipation by Id: {@TournamentParticipation}", id);

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("@GolferId", id.GolferId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@TournamentId", id.TournamentId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Year", id.Year.ToString(), DbType.String, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_TournamentParticipation_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var rowCount = parameters.Get<int>("@RowCount");
        return rowCount > 0;
    }

    public async Task<bool> ExistsByIdAsync(TournamentParticipation id, CancellationToken token = default)
    {
        logger.LogInformation(
            "Checking for existence of TournamentParticipation by Id: {@TournamentParticipation}",
            id);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                """
                SELECT COUNT(1)
                FROM [dbo].[TournamentParticipation]
                WHERE [GolferId] = @GolferId
                    AND [TournamentId] = @TournamentId
                    AND [Year] = @Year;
                """,
                id,
                commandType: CommandType.Text,
                cancellationToken: token));
    }
}