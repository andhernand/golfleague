using GolfLeague.Application.Mapping;
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

    public async Task<GolferResponse?> GetGolferByIdAsync(int id, CancellationToken token = default)
    {
        var golfer = await golferRepository.GetGolferByIdAsync(id, token);
        return golfer?.MapToResponse();
    }

    public async Task<IEnumerable<GolferResponse>> GetAllGolfersAsync(CancellationToken token = default)
    {
        var golfers = await golferRepository.GetAllGolfersAsync(token);
        return golfers.MapToResponse();
    }

    public async Task<GolferResponse?> UpdateAsync(
        int id,
        UpdateGolferRequest request,
        CancellationToken token = default)
    {
        if (id != request.GolferId) return null;

        var golfer = request.MapToGolfer();
        var updated = await golferRepository.UpdateAsync(golfer, token);
        return updated?.MapToResponse();
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
    {
        return await golferRepository.DeleteByIdAsync(id, token);
    }
}