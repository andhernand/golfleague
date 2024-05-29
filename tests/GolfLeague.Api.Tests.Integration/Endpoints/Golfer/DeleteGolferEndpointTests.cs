using System.Net;

using Bogus;

using FluentAssertions;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class DeleteGolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task DeleteGolfer_ReturnsNoContent_WhenGolferIsDeleted()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();

        var createGolferRequest = Fakers.GenerateCreateGolferRequest();
        var createdGolferResponse = await client
            .PostAsJsonAsync(golfApiFactory.GolfersApiBasePath, createGolferRequest);
        var createdGolfer = await createdGolferResponse.Content.ReadFromJsonAsync<GolferResponse>();

        // Act
        var response = await client.DeleteAsync($"{golfApiFactory.GolfersApiBasePath}/{createdGolfer!.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGolfer_ReturnsNotFound_WhenGolferDoesNotExist()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int golferId = Fakers.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{golfApiFactory.GolfersApiBasePath}/{golferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}