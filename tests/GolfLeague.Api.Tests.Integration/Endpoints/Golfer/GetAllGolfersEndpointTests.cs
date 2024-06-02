using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public abstract class GetAllGolfersEndpointTests
{
    public class GetAllGolfersWithTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_WhenGolfersExist_ShouldReturnGolfers()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
            var createdGolfer = await Mother.CreateGolferAsync(client);
            var createdTournament = await Mother.CreateTournamentAsync(client);
            var createdTournamentParticipation = await Mother.CreateTournamentParticipationAsync(
                client,
                createdGolfer.GolferId,
                createdTournament.TournamentId);

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
                                Year = createdTournamentParticipation.Year
                            }
                        }
                    }
                }
            };

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtGenerator.GenerateToken());

            // Act
            var response = await client.GetAsync(Mother.GolfersApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
            golfers.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllGolfersWithNoTournaments(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_WhenGolfersWithNoTournamentsExist_ShouldReturnGolfersWithoutTournaments()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true);
            var createdGolfer = await Mother.CreateGolferAsync(client);

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
                        Tournaments = Array.Empty<TournamentDetailResponse>()
                    }
                }
            };

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtGenerator.GenerateToken());

            // Act
            var response = await client.GetAsync(Mother.GolfersApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var golfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
            golfers.Should().BeEquivalentTo(expected);
        }
    }

    public class GetAllGolfersNoGolfers(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
    {
        [Fact]
        public async Task GetAllGolfers_WhenNoGolfersExist_ShouldReturnNoGolfers()
        {
            // Arrange
            using var client = Mother.CreateAuthorizedClient(golfApiFactory);

            // Act
            var response = await client.GetAsync(Mother.GolfersApiBasePath);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedGolfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
            returnedGolfers!.Golfers.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllGolfers_WhenClientIsUnauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            using var client = golfApiFactory.CreateClient();

            // Act
            var response = await client.GetAsync(Mother.GolfersApiBasePath);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}