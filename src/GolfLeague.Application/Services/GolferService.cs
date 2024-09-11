using GolfLeague.Application.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Repositories;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Services;

public class GolferService(IGolferRepository golferRepository) : IGolferService
{
    public async Task<GolferResponse> CreateAsync(CreateGolferRequest request, CancellationToken token = default)
    {
        var golferRequest = request.MapToGolfer();
        var golfer = await golferRepository.CreateAsync(golferRequest, token);
        return golfer.MapToResponse();
    }

    public async Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token = default)
    {
        return await golferRepository.GetGolferByIdAsync(id, token);
    }

    public async Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token = default)
    {
        return await golferRepository.GetAllGolfersAsync(token);
    }

    public async Task<Golfer?> UpdateAsync(Golfer golfer, CancellationToken token = default)
    {
        var golferExists = await golferRepository.ExistsByIdAsync(golfer.GolferId, token);
        if (!golferExists)
        {
            return null;
        }

        await golferRepository.UpdateAsync(golfer, token);
        return golfer;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        return await golferRepository.DeleteByIdAsync(id, token);
    }
}