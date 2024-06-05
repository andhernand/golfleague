using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IGolferService
{
    Task<int> CreateAsync(Golfer golfer, CancellationToken token = default);
    Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token = default);
    Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token = default);
    Task<Golfer?> UpdateAsync(Golfer golfer, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
}