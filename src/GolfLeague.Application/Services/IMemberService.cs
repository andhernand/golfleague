using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IMemberService
{
    Task<int> CreateAsync(Member member, CancellationToken token = default);
    // Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default);
    // Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default);
}