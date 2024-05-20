using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default);
    Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default);
    Task<Member> Create(Member member, CancellationToken token = default);
}