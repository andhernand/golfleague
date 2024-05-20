namespace GolfLeague.Contracts.Requests;

public class CreateMemberRequest
{
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public required int MemberTypeId { get; init; }
    public string? Phone { get; init; }
    public int? Handicap { get; init; }
    public required DateTime JoinDate { get; init; }
    public int? Coach { get; init; }
    public required char Gender { get; init; }
}