using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class GetTournamentParticipationByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task GetTournamentParticipationById_ReturnsTournamentParticipation_WhenTournamentParticipationExists()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createGolferResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
        var expectedGolfer = await createGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournamentResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
        var expectedTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var expectedYear = Fakers.GenerateYear();

        var createParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = expectedGolfer!.GolferId,
            TournamentId = expectedTournament!.TournamentId,
            Year = expectedYear
        };

        var createdTournamentParticipationResponse = await httpClient
            .PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, createParticipationRequest);
        var createdTournamentParticipation = await createdTournamentParticipationResponse.Content
            .ReadFromJsonAsync<TournamentParticipationResponse>();

        // Act
        var response = await httpClient.GetAsync(
            $"{golfApiFactory.TournamentParticipationsApiBasePath}"
            + $"?golferId={createdTournamentParticipation!.GolferId}"
            + $"&tournamentId={createdTournamentParticipation.TournamentId}"
            + $"&year={createdTournamentParticipation.Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();

        actual.Should().BeEquivalentTo(createdTournamentParticipation);
    }

    [Fact]
    public async Task GetTournamentParticipationById_ReturnsNotFound_WhenTournamentParticipationDoesNotExists()
    {
        // Arrange
        using var httpClient = golfApiFactory.CreateClient();

        var golferId = Fakers.GeneratePositiveInteger();
        var tournamentId = Fakers.GeneratePositiveInteger();
        var year = Fakers.GenerateYear();

        // Act
        var response = await httpClient.GetAsync(
            $"{golfApiFactory.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}