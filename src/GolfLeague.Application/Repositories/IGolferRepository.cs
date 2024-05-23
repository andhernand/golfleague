using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface IGolferRepository
{
    Task<int> CreateAsync(Golfer golfer, CancellationToken token = default);
    Task<Golfer?> GetGolferByIdAsync(int id, CancellationToken token);
    Task<IEnumerable<Golfer>> GetAllGolfersAsync(CancellationToken token);
    Task<bool> UpdateAsync(Golfer golfer, CancellationToken token);
    Task<bool> DeleteByIdAsync(int id, CancellationToken token);
    Task<bool> ExistsByIdAsync(int id, CancellationToken token);
    Task<Golfer?> ExistsByEmailAsync(string email, CancellationToken token = default);
}