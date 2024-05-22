using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IGolferService
{
    Task<int> CreateAsync(Golfer golfer, CancellationToken token = default);
    Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token);
    Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token);
}