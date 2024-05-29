using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public class GetAllTournamentsEndpointTests
{
    public class GetAllGolfersWithTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllTournaments_ReturnTournaments_WhenTournamentsExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            var createGolferRequest = Fakers.GenerateCreateGolferRequest();
            var createdGolferResponse = await client
                .PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
            var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

            var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
            var createdTournamentResponse = await client
                .PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
            var createdTournament = await createdTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

            var createTournamentParticipationRequest = new CreateTournamentParticipationsRequest
            {
                GolferId = createdGolfer!.GolferId,
                TournamentId = createdTournament!.TournamentId,
                Year = Fakers.GenerateYear()
            };
            var createTournamentParticipationResponse = await client
                .PostAsJsonAsync(
                    golfApiFactory.TournamentParticipationsApiBasePath,
                    createTournamentParticipationRequest);
            var createdTournamentParticipation = await createTournamentParticipationResponse.Content
                .ReadFromJsonAsync<TournamentParticipationResponse>();

            var expected = new TournamentsResponse
            {
                Tournaments = new[]
                {
                    new TournamentResponse
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
                    }
                }
            };

            // Act
            var response = await client.GetAsync(golfApiFactory.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
            tournaments.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllTournamentsWithNoParticipants(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllTournaments_ReturnTournamentsWithNoParticipants_WhenTournamentsWithNoParticipantsExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            var createTournamentRequest = Fakers.GenerateCreateTournamentRequest();
            var createTournamentResponse =
                await client.PostAsJsonAsync(golfApiFactory.TournamentsApiBasePath, createTournamentRequest);
            var createdTournament = await createTournamentResponse.Content.ReadFromJsonAsync<TournamentResponse>();

            var expectedTournaments = new TournamentsResponse
            {
                Tournaments = new[]
                {
                    new TournamentResponse
                    {
                        TournamentId = createdTournament!.TournamentId,
                        Name = createdTournament.Name,
                        Format = createdTournament.Format,
                        Participants = []
                    }
                }
            };

            // Act
            var response = await client.GetAsync(golfApiFactory.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
            tournaments.Should().BeEquivalentTo(expectedTournaments);
        }
    }

    public class GetTournamentsNoTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllTournaments_ReturnsNoTournaments_WhenNoTournamentsExist()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            var expectedTournaments = new TournamentsResponse { Tournaments = [] };

            // Act
            var response = await client.GetAsync(golfApiFactory.TournamentsApiBasePath);
            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tournaments.Should().BeEquivalentTo(expectedTournaments);
        }
    }
}