using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface ITournamentParticipationService
{
    Task<bool> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default);

    Task<TournamentParticipation?> GetTournamentParticipationById(
        TournamentParticipation id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default);
    Task<TournamentParticipation?> UpdateAsync(UpdateTournamentParticipation update, CancellationToken token = default);
}