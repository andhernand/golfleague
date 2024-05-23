using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetGolferByIdEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];

    [Fact]
    public async Task GetGolferById_ReturnsGolfer_WhenGolferExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        // Act
        var response = await client.GetAsync($"/api/golfers/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var golfer = await response.Content.ReadFromJsonAsync<GolferResponse>();
        golfer!.GolferId.Should().Be(createdGolfer.GolferId);
        golfer.FirstName.Should().Be(createdGolfer.FirstName);
        golfer.LastName.Should().Be(createdGolfer.LastName);
        golfer.Email.Should().Be(createdGolfer.Email);
        golfer.Handicap.Should().Be(createdGolfer.Handicap);
        golfer.JoinDate.Date.Should().Be(createdGolfer.JoinDate.Date);
    }

    [Fact]
    public async Task GetGolferById_ReturnsNotFound_WhenGolferDoesNotExists()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        // Act
        var result = await client.GetAsync($"/api/golfers/{1000000}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using HttpClient httpClient = golfApiFactory.CreateClient();
        foreach (int golferId in _createdGolferIds)
        {
            await httpClient.DeleteAsync($"/api/golfers/{golferId}");
        }
    }
}