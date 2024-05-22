using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class MemberValidator : AbstractValidator<Member>
{
    private readonly IMemberRepository _memberRepository;

    public MemberValidator(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;

        RuleFor(m => m.FirstName)
            .NotEmpty();

        RuleFor(m => m.LastName)
            .NotEmpty();

        RuleFor(m => m.Email)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .DependentRules(() =>
            {
                RuleFor(m => m.Email)
                    .MustAsync(ValidateEmail)
                    .WithMessage("This Email already exists in the system.")
                    .When(m => m.MemberId != default && m.MemberId > 0);
            });

        RuleFor(m => m.JoinDate)
            .NotEmpty();

        RuleFor(m => m.Handicap)
            .GreaterThanOrEqualTo(0)
            .When(m => m.Handicap is not null);
    }

    private async Task<bool> ValidateEmail(Member member, string email, CancellationToken token = default)
    {
        var exists = await _memberRepository.ExistsByEmailAsync(email, token);
        return !exists;
    }
}