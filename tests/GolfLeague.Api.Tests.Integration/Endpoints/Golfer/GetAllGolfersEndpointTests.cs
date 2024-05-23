using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class GetAllGolfersEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];
    private readonly string _getAllFolfersEndpoint = "/api/golfers";
    private readonly string _createGolferEndpoint = "/api/golfers";

    [Fact]
    public async Task GetAllMemberTypes_ReturnAllMemberTypes_WhenMemberTypesExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client.PostAsJsonAsync(_createGolferEndpoint, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(createdGolfer!.GolferId);

        // Act
        var response = await client.GetAsync(_getAllFolfersEndpoint);

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
        var response = await client.GetAsync(_getAllFolfersEndpoint);
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
            await httpClient.DeleteAsync($"{_getAllFolfersEndpoint}/{golferId}");
        }
    }
}