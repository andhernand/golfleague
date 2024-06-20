using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

using Serilog;

namespace GolfLeague.Application.Repositories;

public class TournamentParticipationRepository(IDbConnectionFactory connectionFactory)
    : ITournamentParticipationRepository
{
    private readonly ILogger _logger = Log.ForContext<TournamentParticipationRepository>();

    public async Task<bool> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default)
    {
        _logger.Information("Creating {@TournamentParticipation}", tournamentParticipation);

        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@GolferId", tournamentParticipation.GolferId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@TournamentId", tournamentParticipation.TournamentId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Year", tournamentParticipation.Year.ToString(), DbType.String, ParameterDirection.Input);
        parameters.Add("@Score", tournamentParticipation.Score, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.usp_TournamentParticipation_Insert",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token));

        var rowCount = parameters.Get<int>("@RowCount");
        return rowCount > 0;
    }

    public async Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default)
    {
        _logger.Information("Deleting {@TournamentParticipation}", id);

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
        _logger.Information(
            "Checking for the existence of TournamentParticipation with {@TournamentParticipation}",
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
                new { id.GolferId, id.TournamentId, id.Year },
                commandType: CommandType.Text,
                cancellationToken: token));
    }
}