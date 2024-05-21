using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IMemberTypeService
{
    Task<int> CreateAsync(MemberType memberType, CancellationToken token = default);
    Task<MemberType?> GetMemberTypeByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<MemberType>> GetAllMemberTypesAsync(CancellationToken token = default);
    Task<MemberType?> GetMemberTypeByNameAsync(string name, CancellationToken token = default);
    Task<MemberType?> UpdateAsync(MemberType memberType, CancellationToken token = default);
}