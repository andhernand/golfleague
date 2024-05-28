using Bogus;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;

namespace GolfLeague.Api.Tests.Integration;

public static class Fakers
{
    public static CreateGolferRequest GenerateCreateGolferRequest(
        string? firstName = default,
        string? lastName = default,
        string? email = default,
        DateOnly? joinDate = default,
        int? handicap = default)
    {
        return new Faker<CreateGolferRequest>()
            .RuleFor(r => r.FirstName, f => firstName ?? f.Person.FirstName)
            .RuleFor(r => r.LastName, f => lastName ?? f.Person.LastName)
            .RuleFor(r => r.Email, f => email ?? f.Person.Email)
            .RuleFor(r => r.JoinDate, f => joinDate ?? DateOnly.FromDateTime(f.Date.Past(20)))
            .RuleFor(r => r.Handicap, f => handicap ?? f.Random.Int(0, 54).OrNull(f))
            .Generate();
    }

    public static UpdateGolferRequest GenerateUpdateGolferRequest(
        string? firstName = default,
        string? lastName = default,
        string? email = default,
        DateOnly? joinDate = default,
        int? handicap = default)
    {
        return new Faker<UpdateGolferRequest>()
            .RuleFor(r => r.FirstName, f => firstName ?? f.Person.FirstName)
            .RuleFor(r => r.LastName, f => lastName ?? f.Person.LastName)
            .RuleFor(r => r.Email, f => email ?? f.Person.Email)
            .RuleFor(r => r.JoinDate, f => joinDate ?? DateOnly.FromDateTime(f.Date.Past(20)))
            .RuleFor(r => r.Handicap, f => handicap ?? f.Random.Int(0, 54).OrNull(f))
            .Generate();
    }

    public static CreateTournamentRequest GenerateCreateTournamentRequest(
        string? name = default,
        string? format = default)
    {
        return new Faker<CreateTournamentRequest>()
            .RuleFor(r => r.Name, f => name ?? f.Company.CompanyName())
            .RuleFor(r => r.Format, f => format ?? f.PickRandom(TournamentFormat.Values))
            .Generate();
    }

    public static UpdateTournamentRequest GenerateUpdateTournamentRequest(
        string? name = default,
        string? format = default)
    {
        return new Faker<UpdateTournamentRequest>()
            .RuleFor(r => r.Name, f => name ?? f.Company.CompanyName())
            .RuleFor(r => r.Format, f => format ?? f.PickRandom(TournamentFormat.Values))
            .Generate();
    }

    public static int GeneratePositiveInteger(int min = 1, int max = int.MaxValue)
    {
        return new Faker().Random.Int(min, max);
    }

    public static int GenerateYear(int yearsBack = 20)
    {
        return new Faker().Date.Past(yearsBack).Year;
    }
}