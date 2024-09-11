namespace GolfLeague.Contracts.Requests;

public record UpdateGolferRequest
{
    public required int GolferId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateOnly JoinDate { get; init; }
    public int? Handicap { get; init; }
}