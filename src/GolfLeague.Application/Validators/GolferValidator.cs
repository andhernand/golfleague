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
                    .WithMessage("This Email already exists in the system.");
            });

        RuleFor(m => m.JoinDate)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(m => m.JoinDate.Year)
                    .LessThanOrEqualTo(DateTime.UtcNow.Year)
                    .OverridePropertyName("JoinDate");
            });

        RuleFor(m => m.Handicap)
            .InclusiveBetween(0, 54)
            .When(m => m.Handicap is not null);
    }

    private async Task<bool> ValidateEmail(Golfer golfer, string email, CancellationToken token = default)
    {
        var existingGolfer = await _golferRepository.ExistsByEmailAsync(email, token);
        if (existingGolfer is not null)
        {
            return existingGolfer.GolferId == golfer.GolferId;
        }

        return existingGolfer is null;
    }
}