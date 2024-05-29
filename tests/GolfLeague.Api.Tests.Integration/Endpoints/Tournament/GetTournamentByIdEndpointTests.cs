using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetTournamentByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task GetTournamentById_ReturnsTournament_WhenTournamentExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createTournament =
            await client.PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
        var createdTournament = await createTournament.Content.ReadFromJsonAsync<TournamentResponse>();

        var expectedTournament = new TournamentResponse
        {
            TournamentId = createdTournament!.TournamentId,
            Name = createdTournament.Name,
            Format = createdTournament.Format,
            Participants = []
        };

        // Act
        var response =
            await client.GetAsync($"{golfApiFactory.TournamentsApiBasePath}/{createdTournament!.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }

    [Fact]
    public async Task GetTournamentById_ReturnsNotFound_WhenTournamentDoesNotExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int tournamentId = Fakers.GeneratePositiveInteger(999_999, 9_999_999);

        // Act
        var result = await client.GetAsync($"{golfApiFactory.TournamentsApiBasePath}/{tournamentId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGolferById_ReturnsGolferWithTournaments_WhenGolferWithTournamentsExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse =
            await client.PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createdTournamentResponse =
            await client.PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
        var createdTournament = await createdTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var createTournamentParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = createdGolfer!.GolferId,
            TournamentId = createdTournament!.TournamentId,
            Year = Fakers.GenerateYear()
        };
        var createTournamentParticipationResponse = await client
            .PostAsJsonAsync(golfApiFactory.TournamentParticipationsApiBasePath, createTournamentParticipationRequest);
        var createdTournamentParticipation = await createTournamentParticipationResponse.Content
            .ReadFromJsonAsync<TournamentParticipationResponse>();

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
                    Year = createdTournamentParticipation!.Year
                }
            }
        };

        // Act
        var response = await client
            .GetAsync($"{golfApiFactory.TournamentsApiBasePath}/{createdTournament.TournamentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tournament = await response.Content.ReadFromJsonAsync<TournamentResponse>();
        tournament.Should().BeEquivalentTo(expectedTournament);
    }
}