using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Validators;

public class TournamentParticipationValidator : AbstractValidator<TournamentParticipation>
{
    private readonly ITournamentParticipationRepository _repository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IGolferRepository _golferRepository;

    public TournamentParticipationValidator(
        ITournamentParticipationRepository repository,
        ITournamentRepository tournamentRepository,
        IGolferRepository golferRepository)
    {
        _repository = repository;
        _tournamentRepository = tournamentRepository;
        _golferRepository = golferRepository;

        RuleFor(x => x.GolferId)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x.GolferId)
                    .MustAsync(ValidateGolferIdAsync)
                    .OverridePropertyName("GolferId")
                    .WithMessage("'Golfer Id' does not exists in the system");
            });

        RuleFor(x => x.TournamentId)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x.TournamentId)
                    .MustAsync(ValidateTournamentIdAsync)
                    .OverridePropertyName("TournamentId")
                    .WithMessage("'Tournament Id' does not exists in the system");
            });

        RuleFor(x => x.Year)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x.Year)
                    .InclusiveBetween(1916, DateTime.UtcNow.Year);
            });

        RuleFor(x => x)
            .MustAsync(ValidateTournamentParticipationAsync)
            .OverridePropertyName("TournamentParticipation")
            .WithMessage("TournamentParticipation already exists in the system.")
            .When(x => x.GolferId != default && x.TournamentId != default && x.Year != default);
    }

    private async Task<bool> ValidateGolferIdAsync(int golferId, CancellationToken token = default)
    {
        var exists = await _golferRepository.ExistsByIdAsync(golferId, token);
        return exists;
    }

    private async Task<bool> ValidateTournamentIdAsync(int tournamentId, CancellationToken token = default)
    {
        var exists = await _tournamentRepository.ExistsByIdAsync(tournamentId, token);
        return exists;
    }

    private async Task<bool> ValidateTournamentParticipationAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken cancellationToken = default)
    {
        var exists = await _repository.ExistsByIdAsync(tournamentParticipation, cancellationToken);
        return !exists;
    }
}