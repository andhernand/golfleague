using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class MemberTypeService(IMemberTypeRepository memberTypeRepository, IValidator<MemberType> validator) : IMemberTypeService
{
    public async Task<int> CreateAsync(MemberType memberType, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(memberType, token);
        return await memberTypeRepository.CreateAsync(memberType, token);
    }

    public async Task<MemberType?> GetMemberTypeByIdAsync(int id, CancellationToken token = default)
    {
        return await memberTypeRepository.GetMemberTypeByIdAsync(id, token);
    }

    public async Task<IEnumerable<MemberType>> GetAllMemberTypesAsync(CancellationToken token = default)
    {
        return await memberTypeRepository.GetAllMemberTypesAsync(token);
    }
}