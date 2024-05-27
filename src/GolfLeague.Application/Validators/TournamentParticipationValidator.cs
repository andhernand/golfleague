using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class TournamentParticipationValidator : AbstractValidator<TournamentParticipation>
{
    private readonly ITournamentParticipationRepository _repository;

    public TournamentParticipationValidator(ITournamentParticipationRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.GolferId)
            .NotEmpty();

        RuleFor(x => x.TournamentId)
            .NotEmpty();

        RuleFor(x => x.Year)
            .NotEmpty();

        RuleFor(x => x)
            .MustAsync(ValidateTournamentParticipationAsync)
            .OverridePropertyName("TournamentParticipation")
            .WithMessage("TournamentParticipation already exists in the system.")
            .When(x => x.GolferId != default && x.TournamentId != default && x.Year != default);
    }

    private async Task<bool> ValidateTournamentParticipationAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken cancellationToken = default)
    {
        return !await _repository.ExistsByIdAsync(tournamentParticipation, cancellationToken);
    }
}