using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class GolferValidator : AbstractValidator<Golfer>
{
    private readonly IGolferRepository _golferRepository;

    public GolferValidator(IGolferRepository golferRepository)
    {
        _golferRepository = golferRepository;

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
                    .When(m => m.GolferId != default && m.GolferId > 0);
            });

        RuleFor(m => m.JoinDate)
            .NotEmpty();

        RuleFor(m => m.Handicap)
            .GreaterThanOrEqualTo(0)
            .When(m => m.Handicap is not null);
    }

    private async Task<bool> ValidateEmail(Golfer golfer, string email, CancellationToken token = default)
    {
        var exists = await _golferRepository.ExistsByEmailAsync(email, token);
        return !exists;
    }
}