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
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.MemberTypeId > 0);

        RuleFor(mt => mt.Name)
            .NotEmpty()
            .DependentRules(() =>
                RuleFor(mt => mt.Name)
                    .MustAsync(ValidateName)
                    .WithMessage("This Member Type already exists in the system")
            )
            .When(x => x.MemberTypeId == 0);
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