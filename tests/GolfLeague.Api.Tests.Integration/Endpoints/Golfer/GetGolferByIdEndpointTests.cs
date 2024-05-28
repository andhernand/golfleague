using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetGolferByIdEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    private const string GolfersApiBasePath = "/api/golfers";

    [Fact]
    public async Task GetGolferById_ReturnsGolfer_WhenGolferExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync(GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        // Act
        var response = await client.GetAsync($"{GolfersApiBasePath}/{createdGolfer!.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer!.GolferId.Should().Be(createdGolfer.GolferId);
        golfer.FirstName.Should().Be(createdGolfer.FirstName);
        golfer.LastName.Should().Be(createdGolfer.LastName);
        golfer.Email.Should().Be(createdGolfer.Email);
        golfer.Handicap.Should().Be(createdGolfer.Handicap);
        golfer.JoinDate.Should().Be(createdGolfer.JoinDate);
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
}