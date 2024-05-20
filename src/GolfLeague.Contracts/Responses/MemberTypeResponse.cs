namespace GolfLeague.Contracts.Responses;

public class MemberTypeResponse
{
    public required int MemberTypeId { get; init; }
    public required string Name { get; init; }
    public decimal? Fee { get; init; }
}