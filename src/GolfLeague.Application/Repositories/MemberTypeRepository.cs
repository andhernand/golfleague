using Dapper;

using GolfLeague.Application.Database;
using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public class MemberTypeRepository(IDbConnectionFactory connectionFactory) : IMemberTypeRepository
{
    public async Task<int> CreateAsync(MemberType memberType, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var insertedMemberTypeId = await connection.QuerySingleAsync<int>(
            new CommandDefinition(
                "dbo.usp_MemberType_Insert",
                new { name = memberType.Name, fee = memberType.Fee },
                cancellationToken: token));
        return insertedMemberTypeId;
    }

    public async Task<MemberType?> GetMemberTypeByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var memberType = await connection.QuerySingleOrDefaultAsync<MemberType>(
            new CommandDefinition(
                "dbo.usp_MemberType_GetByMemberTypeId",
                new { memberTypeId = id },
                cancellationToken: token));
        return memberType;
    }
}