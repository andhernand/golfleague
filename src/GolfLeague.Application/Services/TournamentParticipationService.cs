using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class TournamentParticipationService(
    ITournamentParticipationRepository repository,
    IValidator<TournamentParticipation> validator)
    : ITournamentParticipationService
{
    public async Task<bool> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(tournamentParticipation, token);
        return await repository.CreateAsync(tournamentParticipation, token);
    }

    public async Task<TournamentParticipation?> GetTournamentParticipationById(
        TournamentParticipation id,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetTournamentParticipationById(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }
}