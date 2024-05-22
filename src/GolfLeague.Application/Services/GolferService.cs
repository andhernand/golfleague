using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class GolferService(IGolferRepository golferRepository, IValidator<Golfer> validator) : IGolferService
{
    public async Task<int> CreateAsync(Golfer golfer, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(golfer, token);
        return await golferRepository.Create(golfer, token);
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