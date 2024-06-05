using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface ITournamentParticipationRepository
{
    Task<bool> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default);

    Task<TournamentParticipation?> GetTournamentParticipationById(
        TournamentParticipation id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(TournamentParticipation id, CancellationToken token = default);
    Task<bool> UpdateAsync(UpdateTournamentParticipation update, CancellationToken token = default);
}