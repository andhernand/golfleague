using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public interface ITournamentService
{
    Task<TournamentResponse> CreateAsync(CreateTournamentRequest request, CancellationToken token = default);
    Task<Tournament?> GetTournamentByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<TournamentResponse>> GetAllTournamentsAsync(CancellationToken token = default);
    Task<Tournament?> UpdateAsync(Tournament tournament, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
}