using FluentValidation;

using GolfLeague.Application.Models;

namespace GolfLeague.Application.Validators;

public class MemberTypeValidator : AbstractValidator<MemberType>
{
    public MemberTypeValidator()
    {
        // TODO: add the ability to check for an existing name. Once the Get by Name endpoint is implemented.
        RuleFor(mt => mt.Name)
            .NotEmpty();
    }
}