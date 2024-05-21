namespace GolfLeague.Contracts.Requests;

public class UpdateMemberTypeRequest
{
    public required string Name { get; init; }
    public decimal? Fee { get; init; }
}