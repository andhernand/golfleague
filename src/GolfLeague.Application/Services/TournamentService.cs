using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class TournamentService(ITournamentRepository repository, IValidator<Tournament> validator) : ITournamentService
{
    public async Task<int> CreateAsync(Tournament tournament, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(tournament, token);
        return await repository.CreateAsync(tournament, token);
    }

    public async Task<Tournament?> GetTournamentByIdAsync(int id, CancellationToken token = default)
    {
        return await repository.GetTournamentByIdAsync(id, token);
    }

    public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync(CancellationToken token = default)
    {
        return await repository.GetAllTournamentsAsync(token);
    }

    public async Task<Tournament?> UpdateAsync(Tournament tournament, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(tournament, token);
        bool tournamentExists = await repository.ExistsByIdAsync(tournament.TournamentId, token);
        if (!tournamentExists)
        {
            return null;
        }

        await repository.UpdateAsync(tournament, token);
        return tournament;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token)
    {
        return await repository.DeleteByIdAsync(id, token);
    }
}