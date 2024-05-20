namespace GolfLeague.Contracts.Responses;

public class MemberTypesResponse
{
    public required IEnumerable<MemberTypeResponse> MemberTypes { get; init; } = Enumerable.Empty<MemberTypeResponse>();
}