using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetGolferByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task GetGolferById_WhenGolferExists_ShouldReturnGolfer()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);

        var expected = new GolferResponse
        {
            GolferId = createdGolfer.GolferId,
            FirstName = createdGolfer.FirstName,
            LastName = createdGolfer.LastName,
            Email = createdGolfer.Email,
            Handicap = createdGolfer.Handicap,
            JoinDate = createdGolfer.JoinDate,
            Tournaments = Array.Empty<TournamentDetailResponse>()
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync($"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetGolferById_WhenGolferDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        var id = Mother.GeneratePositiveInteger();

        // Act
        var result = await client.GetAsync($"{Mother.GolfersApiBasePath}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGolferById_WhenGolferWithTournamentsExists_ShouldReturnGolferWithTournaments()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);

        var createdGolfer = await Mother.CreateGolferAsync(client);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        var createdTournamentParticipation = await Mother.CreateGolferTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);

        var expectedGolfer = new GolferResponse
        {
            GolferId = createdGolfer.GolferId,
            FirstName = createdGolfer.FirstName,
            LastName = createdGolfer.LastName,
            Email = createdGolfer.Email,
            Handicap = createdGolfer.Handicap,
            JoinDate = createdGolfer.JoinDate,
            Tournaments = new[]
            {
                new TournamentDetailResponse
                {
                    TournamentId = createdTournament.TournamentId,
                    Name = createdTournament.Name,
                    Format = createdTournament.Format,
                    Year = createdTournamentParticipation.Year
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync($"{Mother.GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expectedGolfer);
    }

    [Fact]
    public async Task GetGolferById_WhenUnAuthenticated_ShouldReturnUnAuthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var id = Mother.GeneratePositiveInteger();

        // Act
        var result = await client.GetAsync($"{Mother.GolfersApiBasePath}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_WhenHealthy_ShouldReturnHealthy()
    {
        using var client = golfApiFactory.CreateClient();

        var response = await client.GetAsync("/_health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("Healthy");
    }
}