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

    public async Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default)
    {
        return await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<TournamentParticipation?> UpdateAsync(
        UpdateTournamentParticipation update,
        CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(update.Update, cancellationToken: token);

        var participationExists = await repository.ExistsByIdAsync(update.Original, token);
        if (!participationExists)
        {
            return null;
        }

        _ = await repository.UpdateAsync(update, token);
        return update.Update;
    }
}