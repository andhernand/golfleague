using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public class GolferRepository(IDbConnectionFactory connectionFactory) : IGolferRepository
{
    public async Task<int> Create(Golfer golfer, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golferId = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "dbo.usp_Golfer_Insert",
                new
                {
                    golfer.FirstName,
                    golfer.LastName,
                    golfer.Email,
                    golfer.JoinDate,
                    golfer.Handicap
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return golferId;
    }

    public async Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token)
    {
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
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var golfers = await connection.QueryAsync<Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return golfers;
    }

    public async Task<Golfer?> ExistsByEmailAsync(string email, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
//         var golfer = await connection.QuerySingleOrDefaultAsync<Golfer>(
//             new CommandDefinition(
//                 """
//                 SET NOCOUNT ON;
//
//                 SELECT [GolferId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
//                 FROM [dbo].[Golfer]
//                 WHERE [Email] = @Email;
//                 """,
//                 new { Email = email },
//                 commandType: CommandType.Text,
//                 cancellationToken: token));
        var golfer = await connection.QuerySingleOrDefaultAsync<Golfer>(
            new CommandDefinition(
                "dbo.usp_Golfer_GetGolferByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));

        return golfer;
    }
}