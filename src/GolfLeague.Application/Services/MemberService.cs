using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default)
    {
        return await memberRepository.GetAllMembersAsync(token);
    }

    public async Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default)
    {
        return await memberRepository.GetMemberByIdAsync(id, token);
    }
}