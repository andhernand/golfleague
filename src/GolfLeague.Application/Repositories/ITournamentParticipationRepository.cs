using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface ITournamentParticipationRepository
{
    Task<TournamentParticipation?> CreateAsync(
        TournamentParticipation tournamentParticipation,
        CancellationToken token = default);

    Task<bool> DeleteAsync(TournamentParticipation id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(TournamentParticipation id, CancellationToken token = default);
}