﻿namespace GolfLeague.Application.Models;

public record TournamentDetail
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}