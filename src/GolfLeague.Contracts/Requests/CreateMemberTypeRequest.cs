namespace GolfLeague.Contracts.Requests;

public class CreateMemberTypeRequest
{
    public required string Name { get; init; }
    public decimal? Fee { get; init; }
}