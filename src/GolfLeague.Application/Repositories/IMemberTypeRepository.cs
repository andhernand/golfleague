using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface IMemberTypeRepository
{
    Task<int> CreateAsync(MemberType memberType, CancellationToken token = default);
    Task<MemberType?> GetMemberTypeByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<MemberType>> GetAllMemberTypesAsync(CancellationToken token = default);
}