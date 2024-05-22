using System.Data;

using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public class MemberRepository(IDbConnectionFactory connectionFactory) : IMemberRepository
{
    public async Task<int> Create(Member member, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var insertedMemberTypeId = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "dbo.usp_Member_Insert",
                member,
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return insertedMemberTypeId;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM [dbo].[Member] WHERE Email = @email;",
                new { email },
                commandType: CommandType.Text,
                cancellationToken: token));
    }

    // public async Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default)
    // {
    //     using var connection = await connectionFactory.CreateConnectionAsync(token);
    //     var members = await connection.QueryAsync<Member>(new CommandDefinition(
    //         "dbo.usp_Member_GetAll",
    //         commandType: CommandType.StoredProcedure,
    //         cancellationToken: token));
    //     return members;
    // }
    //
    // public async Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default)
    // {
    //     using var connection = await connectionFactory.CreateConnectionAsync(token);
    //     var member = await connection.QuerySingleOrDefaultAsync<Member>(new CommandDefinition(
    //         "dbo.usp_Member_GetByMemberId",
    //         new { memberId = id },
    //         commandType: CommandType.StoredProcedure,
    //         cancellationToken: token));
    //     return member;
    // }
    //
}