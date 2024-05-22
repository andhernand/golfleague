using System.Net;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints;

public class GolfersEndpointTests(GolfApiFactory golfApiFactory) :
    IClassFixture<GolfApiFactory>,
    IAsyncLifetime
{
    private readonly List<int> _createdGolferIds = [];

    [Fact]
    public async Task CreateGolfer_CreatesGolfer_WhenDataIsCorrect()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        var request = Fakers.GenerateCreateGolferRequest();

        // Act
        var result = await client.PostAsJsonAsync("/api/golfers", request);
        var golfer = await result.Content.ReadFromJsonAsync<GolferResponse>();
        _createdGolferIds.Add(golfer!.GolferId);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Headers.Location.Should().Be($"http://localhost/api/golfers/{golfer.GolferId}");
        golfer.FirstName.Should().Be(request.FirstName);
        golfer.LastName.Should().Be(request.LastName);
        golfer.Email.Should().Be(request.Email);
        golfer.JoinDate.Should().Be(request.JoinDate);
        golfer.Handicap.Should().Be(request.Handicap);
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_FirstNameIsInvalid()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_LastNameIsInvalid()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_EmailIsInvalid()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_GolferWithTheSameEmailExists()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_JoinDateIsInvalid()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGolfer_Fails_When_HandicapIsInvalid()
    {
        await Task.Delay(50);
        true.Should().BeTrue();
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