using GolfLeague.Application.Models;

namespace GolfLeague.Application.Repositories;

public interface IMemberRepository
{
    Task<IEnumerable<Member>> GetAllMembersAsync(CancellationToken token = default);
}