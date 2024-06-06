using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Tournament;

public abstract class GetAllTournamentsEndpointTests
{
    public class GetAllGolfersWithTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllTournaments_WhenTournamentsExist_ShouldReturnTournaments()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
            var createdGolfer = await Mother.CreateGolferAsync(client);
            var createdTournament = await Mother.CreateTournamentAsync(client);
            var createdTournamentParticipation = await Mother.CreateTournamentGolferParticipationAsync(
                client,
                createdGolfer.GolferId,
                createdTournament.TournamentId);

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
                                Year = createdTournamentParticipation.Year,
                                Score = createdTournamentParticipation.Score
                            }
                        }
                    }
                }
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", JwtGenerator.GenerateToken());

            // Act
            var response = await client.GetAsync(Mother.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
            tournaments.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllTournamentsWithNoParticipants(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task
            GetAllTournaments_WhenTournamentsWithNoParticipantsExist_ShouldReturnTournamentsWithNoParticipants()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
            var createdTournament = await Mother.CreateTournamentAsync(client);

            var expectedTournaments = new TournamentsResponse
            {
                Tournaments = new[]
                {
                    new TournamentResponse
                    {
                        TournamentId = createdTournament.TournamentId,
                        Name = createdTournament.Name,
                        Format = createdTournament.Format,
                        Participants = []
                    }
                }
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", JwtGenerator.GenerateToken());

            // Act
            var response = await client.GetAsync(Mother.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
            tournaments.Should().BeEquivalentTo(expectedTournaments);
        }
    }

    public class GetTournamentsNoTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllTournaments_WhenNoTournamentsExist_ShouldReturnNoTournaments()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory);
            var expectedTournaments = new TournamentsResponse { Tournaments = [] };

            // Act
            var response = await client.GetAsync(Mother.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tournaments = await response.Content.ReadFromJsonAsync<TournamentsResponse>();
            tournaments.Should().BeEquivalentTo(expectedTournaments);
        }

        [Fact]
        public async Task GetAllTournaments_WhenUnauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync(Mother.TournamentsApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}