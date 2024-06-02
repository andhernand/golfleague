using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetTournamentByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task GetTournamentById_WhenTournamentExists_ShouldReturnTournament()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdTournament = await Mother.CreateTournamentAsync(client);

        var expectedTournament = new TournamentResponse
        {
            TournamentId = createdTournament.TournamentId,
            Name = createdTournament.Name,
            Format = createdTournament.Format,
            Participants = []
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }

    [Fact]
    public async Task GetTournamentById_WhenTournamentDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory);
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTournamentById_WhenTournamentWithGolfersExists_ShouldReturnTournamentWithGolfers()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
        var createdGolfer = await Mother.CreateGolferAsync(client);
        var createdTournament = await Mother.CreateTournamentAsync(client);
        var createdTournamentParticipation = await Mother.CreateTournamentParticipationAsync(
            client,
            createdGolfer.GolferId,
            createdTournament.TournamentId);

        var expectedTournament = new TournamentResponse
        {
            TournamentId = createdTournament.TournamentId,
            Name = createdTournament.Name,
            Format = createdTournament.Format,
            Participants = new[]
            {
                new ParticipationDetailResponse
                {
                    GolferId = createdGolfer.GolferId,
                    FirstName = createdGolfer.FirstName,
                    LastName = createdGolfer.LastName,
                    Year = createdTournamentParticipation.Year
                }
            }
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken());

        // Act
        var response = await client.GetAsync(
            $"{Mother.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }

    [Fact]
    public async Task GetTournamentById_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Mother.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{Mother.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}