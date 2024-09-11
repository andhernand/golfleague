using FluentValidation;

using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;

namespace GolfLeague.Application.Validators;

public class CreateGolferRequestValidator : AbstractValidator<CreateGolferRequest>
{
    private readonly IGolferRepository _golferRepository;

    public CreateGolferRequestValidator(IGolferRepository golferRepository)
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

    private async Task<bool> ValidateEmail(CreateGolferRequest golfer, string email, CancellationToken token = default)
    {
        var existingGolfer = await _golferRepository.ExistsByEmailAsync(email, token);
        return existingGolfer is null;
    }
}