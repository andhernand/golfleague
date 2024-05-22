namespace GolfLeague.Contracts.Responses;

public class MemberResponse
{
    public required int? MemberId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateTime JoinDate { get; init; }
    public int? Handicap { get; init; }
}