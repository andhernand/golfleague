namespace GolfLeague.Contracts.Responses;

public class GolfersResponse
{
    public required IEnumerable<GolferResponse> Golfers { get; init; } = [];
}