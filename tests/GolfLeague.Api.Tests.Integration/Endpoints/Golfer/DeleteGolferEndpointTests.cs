using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class DeleteGolferEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];



    [Fact]
    public async Task DeleteMemberType_ReturnsNoContent_WhenMemberTypeIsDeleted()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        // Act
        var response = await client.DeleteAsync($"/api/golfers/{createdGolfer.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteMemberType_ReturnsNotFound_WhenMemberTypeDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int golferId = new Faker().Random.Int(999999);

        // Act
        var response = await client.DeleteAsync($"/api/golfers/{golferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var httpClient = golfApiFactory.CreateClient();
        foreach (var golferId in _createdGolferIds)
        {
            await httpClient.DeleteAsync($"/api/golfers/{golferId}");
        }
    }
}