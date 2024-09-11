using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;

namespace GolfLeague.Application.Validators;

public class UpdateTournamentRequestValidator : AbstractValidator<UpdateTournamentRequest>
{
    private readonly ITournamentRepository _repository;

    public UpdateTournamentRequestValidator(ITournamentRepository repository)
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

    private async Task<bool> ValidateTournament(UpdateTournamentRequest tournament, CancellationToken token = default)
    {
        var existingTournament = await _repository.GetByNameAndFormat(tournament.Name, tournament.Format, token);
        if (existingTournament is not null)
        {
            return tournament.TournamentId == existingTournament.TournamentId;
        }

        return existingTournament is null;
    }
}