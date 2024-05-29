using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetGolferByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private const string GolfersApiBasePath = "/api/golfers";
    private const string TournamentsApiBasePath = "/api/tournaments";
    private const string TournamentParticipationsApiBasePath = "/api/tournamentparticipations";

    [Fact]
    public async Task GetGolferById_ReturnsGolfer_WhenGolferExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var expected = new GolferResponse
        {
            GolferId = createdGolfer!.GolferId,
            FirstName = createdGolfer.FirstName,
            LastName = createdGolfer.LastName,
            Email = createdGolfer.Email,
            Handicap = createdGolfer.Handicap,
            JoinDate = createdGolfer.JoinDate,
            Tournaments = []
        };

        // Act
        var response = await client.GetAsync($"{GolfersApiBasePath}/{createdGolfer!.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetGolferById_ReturnsNotFound_WhenGolferDoesNotExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var id = Fakers.GeneratePositiveInteger();

        // Act
        var result = await client.GetAsync($"{GolfersApiBasePath}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGolferById_ReturnsGolferWithTournaments_WhenGolferWithTournamentsExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
        var createdTournamentResponse = await client.PostAsJsonAsync(TournamentsApiBasePath, createTournamentRequest);
        var createdTournament = await createdTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

        var createTournamentParticipationRequest = new CreateTournamentParticipationsRequest
        {
            GolferId = createdGolfer!.GolferId,
            TournamentId = createdTournament!.TournamentId,
            Year = Fakers.GenerateYear()
        };
        var createTournamentParticipationResponse = await client
            .PostAsJsonAsync(TournamentParticipationsApiBasePath, createTournamentParticipationRequest);
        var createdTournamentParticipation = await createTournamentParticipationResponse.Content
            .ReadFromJsonAsync<TournamentParticipationResponse>();

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
                    Year = createdTournamentParticipation!.Year
                }
            }
        };

        // Act
        var response = await client.GetAsync($"{GolfersApiBasePath}/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer.Should().BeEquivalentTo(expectedGolfer);
    }
}