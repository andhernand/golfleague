using System.Net;

using FluentAssertions;

namespace GolfLeague.Api.Tests.Integration.Endpoints.TournamentParticipation;

public class DeleteTournamentParticipationEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task DeleteTournamentParticipation_WhenTournamentParticipationIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        var createdTournamentParticipation = await Mother.CreateTournamentGolferParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={createdTournamentParticipation.GolferId}"
            + $"&tournamentId={createdTournamentParticipation.TournamentId}"
            + $"&year={createdTournamentParticipation.Year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenTournamentParticipationDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTournamentParticipation_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isTrusted: true);
        var golferId = Mother.GeneratePositiveInteger();
        var tournamentId = Mother.GeneratePositiveInteger();
        var year = Mother.GenerateYear();

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentParticipationsApiBasePath}"
            + $"?golferId={golferId}"
            + $"&tournamentId={tournamentId}"
            + $"&year={year}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}