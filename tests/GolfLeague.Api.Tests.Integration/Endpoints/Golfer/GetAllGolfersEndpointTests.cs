using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetAllGolfersEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];

    [Fact]
    public async Task GetAllMemberTypes_ReturnAllMemberTypes_WhenMemberTypesExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync("/api/golfers", createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        // Act
        var response = await client.GetAsync("/api/golfers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedGolfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();
        returnedGolfers!.Golfers.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllMemberTypes_ReturnsNoMemberTypes_WhenNoMemberTypesExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/golfers");
        var returnedGolfers = await response.Content.ReadFromJsonAsync<GolfersResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedGolfers!.Golfers.Should().BeEmpty();
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