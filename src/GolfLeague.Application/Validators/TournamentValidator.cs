using FluentValidation;

using GolfLeague.Application.Models;

namespace GolfLeague.Application.Validators;

public class TournamentValidator : AbstractValidator<Tournament>
{
    public TournamentValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Format).NotEmpty();
    }
}