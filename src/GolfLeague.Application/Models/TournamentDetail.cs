// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GolfLeague.Application.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class TournamentDetail
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public required int Year { get; init; }
}