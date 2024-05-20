namespace GolfLeague.Application.Models;

public class MemberType
{
    public int MemberTypeId { get; set; }
    public required string Name { get; init; }
    public decimal? Fee { get; init; }
}