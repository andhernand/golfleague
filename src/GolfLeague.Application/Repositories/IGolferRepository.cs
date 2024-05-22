using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface IGolferRepository
{
    Task<int> Create(Golfer golfer, CancellationToken token = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken token = default);
    // Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default);
    // Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default);
}