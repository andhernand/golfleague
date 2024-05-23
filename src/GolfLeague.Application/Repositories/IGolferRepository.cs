using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface IGolferRepository
{
    Task<int> Create(Golfer golfer, CancellationToken token = default);
    Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token);
    Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token);
    Task<Golfer?> ExistsByEmailAsync(string email, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token);
}