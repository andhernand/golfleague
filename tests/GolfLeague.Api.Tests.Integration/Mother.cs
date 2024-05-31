using System.Net.Http.Headers;

using Bogus;

using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

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

    public static async Task<GolferResponse?> CreateGolferAsync(HttpClient client)
    {
        var createGolferRequest = GenerateCreateGolferRequest();
        var response = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        return await response.Content.ReadFromJsonAsync<GolferResponse>();
    }

    public static async Task<TournamentResponse?> CreateTournamentAsync(HttpClient client)
    {
        var createTournamentRequest = GenerateCreateTournamentRequest();
        var response = await client.PostAsJsonAsync(TournamentsApiBasePath, createTournamentRequest);
        return await response.Content.ReadFromJsonAsync<TournamentResponse>();
    }

    public static async Task<TournamentParticipationResponse?> CreateTournamentParticipationAsync(
        HttpClient client,
        int golferId,
        int tournamentId)
    {
        var createTournamentParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = golferId, TournamentId = tournamentId, Year = GenerateYear()
        };

        var response = await client.PostAsJsonAsync(
            TournamentParticipationsApiBasePath, createTournamentParticipationRequest);
        return await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();
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

    public static ValidationFailureResponse CreateValidationFailureResponse(string propertyName, string errorMessage)
    {
        return new ValidationFailureResponse
        {
            Errors = new[] { new ValidationResponse { PropertyName = propertyName, ErrorMessage = errorMessage } }
        };
    }
}