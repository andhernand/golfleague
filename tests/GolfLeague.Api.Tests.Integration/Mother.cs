using System.Net.Http.Headers;

using Bogus;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Tests.Integration;

public static class Mother
{
    public const string GolfersApiBasePath = "/api/golfers";
    public const string TournamentsApiBasePath = "/api/tournaments";
    public const string TournamentParticipationsApiBasePath = "/api/tournamentparticipations";

    public static HttpClient CreateAuthorizedClient(
        GolfApiFactory factory,
        bool isTrusted = false,
        bool isAdmin = false)
    {
        var client = factory.CreateClient();
        var token = JwtGenerator.GenerateToken(isTrusted: isTrusted, isAdmin: isAdmin);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<GolferResponse> CreateGolferAsync(HttpClient client)
    {
        var golferRequest = GenerateCreateGolferRequest();
        var response = await client.PostAsJsonAsync(GolfersApiBasePath, golferRequest);
        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        return golfer!;
    }

    public static async Task<TournamentResponse> CreateTournamentAsync(HttpClient client)
    {
        var tournamentRequest = GenerateCreateTournamentRequest();
        var response = await client.PostAsJsonAsync(TournamentsApiBasePath, tournamentRequest);
        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        return tournament!;
    }

    public static async Task<TournamentParticipationResponse> CreateGolferTournamentParticipationAsync(
        HttpClient client,
        int golferId,
        int tournamentId)
    {
        var request = new CreateGolferTournamentParticipationRequest
        {
            TournamentId = tournamentId, Year = GenerateYear(), Score = GenerateScore()
        };

        var response = await client.PostAsJsonAsync(
            $"{GolfersApiBasePath}/{golferId}/tournamentparticipations",
            request);

        var tournamentParticipation = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
        return tournamentParticipation!;
    }

    public static async Task<TournamentParticipationResponse> CreateTournamentGolferParticipationAsync(
        HttpClient client,
        int golferId,
        int tournamentId)
    {
        var request = new CreateTournamentGolferParticipationRequest
        {
            GolferId = golferId, Year = GenerateYear(), Score = GenerateScore()
        };

        var response = await client.PostAsJsonAsync(
            $"{TournamentsApiBasePath}/{tournamentId}/tournamentparticipations",
            request);

        var tournamentParticipation = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
        return tournamentParticipation!;
    }

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

    public static ValidationProblemDetails CreateValidationProblemDetails(Dictionary<string, string[]> errors)
    {
        return new ValidationProblemDetails { Status = StatusCodes.Status400BadRequest, Errors = errors };
    }

    public static int GenerateScore()
    {
        return new Faker().Random.Int(50, 130);
    }
}