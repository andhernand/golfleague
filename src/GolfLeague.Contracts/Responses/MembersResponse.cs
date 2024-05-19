namespace GolfLeague.Contracts.Responses;

public class MembersResponse
{
    public required IEnumerable<MemberResponse> Members { get; init; } = Enumerable.Empty<MemberResponse>();
}