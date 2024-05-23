using FluentValidation;

using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;

namespace GolfLeague.Application.Services;

public class GolferService(IGolferRepository golferRepository, IValidator<Golfer> validator) : IGolferService
{
    public async Task<int> CreateAsync(Golfer golfer, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(golfer, token);
        return await golferRepository.CreateAsync(golfer, token);
    }

    public async Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token)
    {
        return await golferRepository.GetGolferByIdAsync(id, token);
    }

    public async Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token)
    {
        return await golferRepository.GetAllGolfersAsync(token);
    }

    public async Task<Golfer?> UpdateAsync(Golfer golfer, CancellationToken token)
    {
        await validator.ValidateAndThrowAsync(golfer, token);
        var golferExists = await golferRepository.ExistsByIdAsync(golfer.GolferId, token);
        if (!golferExists)
        {
            return null;
        }

        await golferRepository.UpdateAsync(golfer, token);
        return golfer;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token)
    {
        return await golferRepository.DeleteByIdAsync(id, token);
    }
}