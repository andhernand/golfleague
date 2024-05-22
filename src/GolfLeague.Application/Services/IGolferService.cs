using GolfLeague.Application.Models;

namespace GolfLeague.Application.Services;

public interface IGolferService
{
    Task<int> CreateAsync(Golfer golfer, CancellationToken token = default);
    // Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default);
    // Task<Member?> GetMemberByIdAsync(int id, CancellationToken token = default);
}