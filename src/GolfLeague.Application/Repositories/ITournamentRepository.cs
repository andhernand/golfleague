using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface ITournamentRepository
{
    Task<Tournament> CreateAsync(Tournament tournament, CancellationToken token = default);
    Task<Tournament?> GetTournamentByIdAsync(int tournamentId, CancellationToken token = default);
    Task<Tournament?> GetByNameAndFormat(string name, string format, CancellationToken token = default);
    Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default);
    Task<Tournament?> UpdateAsync(Tournament tournament, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int tournamentId, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(int tournamentId, CancellationToken token = default);
}