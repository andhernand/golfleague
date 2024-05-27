using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class DeleteTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private const string GolfersApiBasePath = "/api/golfers";
    private const string TournamentsApiBasePath = "/api/tournaments";
    private const string TournamentParticipationsApiBasePath = "/api/tournamentparticipations";

    [Fact]
    public async Task DeleteTournamentParticipation_ReturnsNoContent_WhenTournamentParticipationIsDeleted()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createGolferResponse = await httpClient
            .PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        var expectedGolfer = await createGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await httpClient
            .PostAsJsonAsync(TournamentsApiBasePath, createTournamentRequest);
        var expectedTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var expectedYear = new Faker().Date.Past(20).Year;

        var createParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = expectedGolfer!.GolferId,
            TournamentId = expectedTournament!.TournamentId,
            Year = expectedYear
        };

        var createdTournamentParticipationResponse = await httpClient
            .PostAsJsonAsync(TournamentParticipationsApiBasePath, createParticipationRequest);
        var createdTournamentParticipation = await createdTournamentParticipationResponse.Content
            .ReadFromJsonAsync<TournamentParticipationResponse>();

        // Act
        var response = await httpClient.DeleteAsync(
            $"{TournamentParticipationsApiBasePath}"
            + $"?golferId={createdTournamentParticipation!.GolferId}"
            + $"&tournamentId={createdTournamentParticipation.TournamentId}"
            + $"&year={createdTournamentParticipation.Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_ReturnsNotFound_WhenTournamentParticipationDoesNotExist()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var faker = new Faker();
        var golferId = faker.Random.Int(1);
        var tournamentId = faker.Random.Int(1);
        var year = faker.Date.Past(20).Year;

        // Act
        var response = await httpClient.DeleteAsync(
            $"{TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}