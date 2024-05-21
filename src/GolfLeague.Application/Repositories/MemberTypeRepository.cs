using System.Data;

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
                commandType: CommandType.StoredProcedure,
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
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return memberType;
    }

    public async Task<IEnumerable<MemberType>> GetAllMemberTypesAsync(CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var memberTypes = await connection.QueryAsync<MemberType>(
            new CommandDefinition(
                "dbo.usp_MemberType_GetAll",
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return memberTypes;
    }

    public async Task<MemberType?> GetMemberTypeByNameAsync(string name, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var memberType = await connection.QuerySingleOrDefaultAsync<MemberType>(
            new CommandDefinition(
                "dbo.usp_MemberType_GetByName",
                new { name },
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return memberType;
    }

    public async Task<bool> UpdateAsync(MemberType memberType, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(
            new CommandDefinition(
                "dbo.usp_MemberType_Update",
                memberType,
                commandType: CommandType.StoredProcedure,
                cancellationToken: token));
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM [dbo].[MemberType] WHERE MemberTypeId = @id;",
                new { id },
                commandType: CommandType.Text,
                cancellationToken: token));
    }
}