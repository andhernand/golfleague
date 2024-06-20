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

    public Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default)
    {
        return repository.DeleteAsync(id, cancellationToken);
    }
}