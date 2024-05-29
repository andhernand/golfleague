using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetAllGolfersEndpointTests
{
    private const string GolfersApiBasePath = "/api/golfers";
    private const string TournamentsApiBasePath = "/api/tournaments";
    private const string TournamentParticipationsApiBasePath = "/api/tournamentparticipations";

    public class GetAllGolfersWithTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_ReturnGolfers_WhenGolfersExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            var createGolferRequest = Fakers.GenerateCreateGolferRequest();
            var createdGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
            var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

            var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
            var createdTournamentResponse =
                await client.PostAsJsonAsync(TournamentsApiBasePath, createTournamentRequest);
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

            var expected = new GolfersResponse
            {
                Golfers = new[]
                {
                    new GolferResponse
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
                    }
                }
            };

            // Act
            var response = await client.GetAsync(GolfersApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
            golfers.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllGolfersWithNoTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_ReturnGolfersWithoutTournaments_WhenGolfersWithNoTournamentsExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            var createGolferRequest = Fakers.GenerateCreateGolferRequest();
            var createdGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
            var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

            var expected = new GolfersResponse
            {
                Golfers = new[]
                {
                    new GolferResponse
                    {
                        GolferId = createdGolfer!.GolferId,
                        FirstName = createdGolfer.FirstName,
                        LastName = createdGolfer.LastName,
                        Email = createdGolfer.Email,
                        Handicap = createdGolfer.Handicap,
                        JoinDate = createdGolfer.JoinDate,
                        Tournaments = []
                    }
                }
            };

            // Act
            var response = await client.GetAsync(GolfersApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
            golfers.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllGolfersNoGolfers(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_ReturnsNoGolfers_WhenNoGolfersExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync(GolfersApiBasePath);
            var returnedGolfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            returnedGolfers!.Golfers.Should().BeEmpty();
        }
    }
}