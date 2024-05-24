using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface ITournamentService
{
    Task<int> CreateAsync(Tournament tournament, CancellationToken token = default);
    Task<Tournament?> GetTournamentByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default);
    Task<Tournament?> UpdateAsync(Tournament tournament, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token);
}