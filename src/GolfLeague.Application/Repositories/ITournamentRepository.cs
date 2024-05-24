using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface ITournamentRepository
{
    Task<int> CreateAsync(Tournament tournament, CancellationToken token = default);
    Task<Tournament?> GetTournamentByIdAsync(int tournamentId, CancellationToken token = default);
    Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default);
    Task<bool> UpdateAsync(Tournament tournament, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int tournamentId, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(int tournamentId, CancellationToken token = default);
}