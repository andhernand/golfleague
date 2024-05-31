// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GolfLeague.Contracts.Requests;

// ReSharper disable once ClassNeverInstantiated.Global
public class UpdateGolferRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required DateOnly JoinDate { get; init; }
    public int? Handicap { get; init; }
}