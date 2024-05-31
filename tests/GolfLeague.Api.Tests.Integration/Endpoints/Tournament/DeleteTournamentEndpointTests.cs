using System.Net;

using FluentAssertions;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class DeleteTournamentEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task DeleteTournament_WhenTournamentIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        // Act
        var response = await client.DeleteAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament!.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTournament_WhenTournamentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTournament_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTournament_WhenClientDoesNotHaveProperPermissions_ShouldFailWithForbidden()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        int tournamentId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}