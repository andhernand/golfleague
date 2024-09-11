using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;

namespace GolfLeague.Application.Validators;

public class CreateTournamentRequestValidator : AbstractValidator<CreateTournamentRequest>
{
    private readonly ITournamentRepository _repository;

    public CreateTournamentRequestValidator(ITournamentRepository repository)
    {
        _repository = repository;

        RuleFor(t => t.Name)
            .NotEmpty();

        RuleFor(t => t.Format)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(t => t.Format)
                    .Must(t => TournamentFormat.Values.Any(v => v.Equals(t)))
                    .WithMessage($"'Format' must be one of any: {string.Join(", ", TournamentFormat.Values)}");
            });

        RuleFor(t => t)
            .MustAsync(ValidateTournament)
            .OverridePropertyName("Tournament")
            .WithMessage("A Tournament with the Name and Format combination already exists in the system.")
            .When(t => !string.IsNullOrWhiteSpace(t.Name) && !string.IsNullOrWhiteSpace(t.Format));
    }

    private async Task<bool> ValidateTournament(CreateTournamentRequest tournament, CancellationToken token = default)
    {
        var existingTournament = await _repository.GetByNameAndFormat(tournament.Name, tournament.Format, token);
        return existingTournament is null;
    }
}