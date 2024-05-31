// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GolfLeague.Application.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class ParticipationDetail
{
    public required int GolferId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int Year { get; init; }
}