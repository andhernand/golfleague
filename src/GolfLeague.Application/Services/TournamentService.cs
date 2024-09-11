using GolfLeague.Application.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public class TournamentService(ITournamentRepository repository) : ITournamentService
{
    public async Task<TournamentResponse> CreateAsync(
        CreateTournamentRequest request,
        CancellationToken token = default)
    {
        var tournamentRequest = request.MapToTournament();
        var tournament = await repository.CreateAsync(tournamentRequest, token);
        return tournament.MapToResponse();
    }

    public async Task<TournamentResponse?> GetTournamentByIdAsync(int id, CancellationToken token = default)
    {
        var tournament = await repository.GetTournamentByIdAsync(id, token);
        return tournament?.MapToResponse();
    }

    public async Task<IEnumerable<TournamentResponse>> GetAllTournamentsAsync(CancellationToken token = default)
    {
        var tournaments = await repository.GetAllTournamentsAsync(token);
        return tournaments.MapToResponse();
    }

    public async Task<Tournament?> UpdateAsync(Tournament tournament, CancellationToken token = default)
    {
        bool tournamentExists = await repository.ExistsByIdAsync(tournament.TournamentId, token);
        if (!tournamentExists)
        {
            return null;
        }

        await repository.UpdateAsync(tournament, token);
        return tournament;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        return await repository.DeleteByIdAsync(id, token);
    }
}