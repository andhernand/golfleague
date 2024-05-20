using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class MemberTypeValidator : AbstractValidator<MemberType>
{
    private readonly IMemberTypeRepository _memberTypeRepository;
    public MemberTypeValidator(IMemberTypeRepository memberTypeRepository)
    {
        _memberTypeRepository = memberTypeRepository;
        
        RuleFor(mt => mt.Name)
            .NotEmpty();
        
        RuleFor(x => x.Name)
            .MustAsync(ValidateName)
            .WithMessage("This Member Type already exists in the system");
    }
    
    private async Task<bool> ValidateName(MemberType memberType, string name, CancellationToken token = default)
    {
        var existingMemberType = await _memberTypeRepository.GetMemberTypeByNameAsync(name, token);

        if (existingMemberType is not null)
        {
            return existingMemberType.MemberTypeId == memberType.MemberTypeId;
        }

        return existingMemberType is null;
    }
}