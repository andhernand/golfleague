using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class MemberService(IMemberRepository memberRepository, IValidator<Member> validator) : IMemberService
{
    public async Task<int> CreateAsync(Member member, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(member, token);
        return await memberRepository.Create(member, token);
    }
    // public async Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default)
    // {
    //     return await memberRepository.GetAllMembersAsync(token);
    // }
    //
    // public async Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default)
    // {
    //     return await memberRepository.GetMemberByIdAsync(id, token);
    // }
    //
}