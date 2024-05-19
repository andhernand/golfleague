using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public class MemberRepository(IDbConnectionFactory connectionFactory) : IMemberRepository
{
    public async Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(token);
        var members = await connection.QueryAsync<Member>(new CommandDefinition(
            "dbo.usp_Member_GetAll",
            commandType: CommandType.StoredProcedure,
            cancellationToken: token));
        return members;
    }
}