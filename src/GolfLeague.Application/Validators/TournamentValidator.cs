using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class TournamentValidator : AbstractValidator<Tournament>
{
    private readonly ITournamentRepository _repository;

    public TournamentValidator(ITournamentRepository repository)
    {
        _repository = repository;

        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Format).NotEmpty();

        RuleFor(t => t)
            .MustAsync(ValidateTournament)
            .OverridePropertyName("Tournament")
            .WithMessage("A Tournament with the Name and Format combination already exists in the system.")
            .When(t => !string.IsNullOrWhiteSpace(t.Name) && !string.IsNullOrWhiteSpace(t.Format));
    }

    private async Task<bool> ValidateTournament(Tournament tournament, CancellationToken token = default)
    {
        var existingTournament = await _repository.GetByNameAndFormat(tournament.Name, tournament.Format, token);
        if (existingTournament is not null)
        {
            return tournament.TournamentId == existingTournament.TournamentId;
        }

        return existingTournament is null;
    }
}