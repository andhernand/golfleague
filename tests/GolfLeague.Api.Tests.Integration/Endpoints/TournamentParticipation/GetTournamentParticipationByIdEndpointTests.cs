using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class GetTournamentParticipationByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task
        GetTournamentParticipationById_WhenTournamentParticipationExists_ShouldReturnTournamentParticipation()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        var createdTournamentParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={createdTournamentParticipation.GolferId}"
            + $"&tournamentId={createdTournamentParticipation.TournamentId}"
            + $"&year={createdTournamentParticipation.Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<TournamentParticipationResponse>();

        actual.Should().BeEquivalentTo(createdTournamentParticipation);
    }

    [Fact]
    public async Task GetTournamentParticipationById_WhenTournamentParticipationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTournamentParticipationById_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);
        int golferId = Mother.GeneratePositiveInteger(999_999, 9_999_999);
        int year = Mother.GenerateYear();

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}