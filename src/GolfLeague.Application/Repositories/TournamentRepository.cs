using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public class TournamentRepository(IDbConnectionFactory connectionFactory) : ITournamentRepository
{
    public async Task<int> CreateAsync(Tournament tournament, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        
        var tournamentId = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "dbo.usp_Tournament_Insert",
                new { tournament.Name, tournament.Format },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return tournamentId;
    }

    public async Task<Tournament?> GetTournamentByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var tournament = await connection.QueryFirstOrDefaultAsync<Tournament>(
            new CommandDefinition(
                "dbo.usp_Tournament_GetTournamentById",
                new { TournamentId = id },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return tournament;
    }

    public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var tournaments = await connection.QueryAsync<Tournament>(
            new CommandDefinition(
                "dbo.usp_Tournament_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return tournaments;
    }

    public async Task<bool> UpdateAsync(Tournament tournament, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_Tournament_Update",
                tournament,
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(int tournamentId, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        var parameters = new DynamicParameters();
        parameters.Add("@TournamentId", tournamentId, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@RowCount", 0, DbType.Int32, ParameterDirection.Output);

        var commandDefinition = new CommandDefinition(
            "dbo.usp_Tournament_Delete",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: token);

        await connection.ExecuteScalarAsync(commandDefinition);

        var result = parameters.Get<int>("@RowCount") > 0;
        return result;
    }

    public async Task<bool> ExistsByIdAsync(int tournamentId, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM [dbo].[Tournament] WHERE TournamentId = @tournamentId;",
                new { tournamentId },
                commandType: CommandType.Text,
                cancellationToken: token));
    }
}