using System.Net;

using FluentAssertions;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Golfer;

public class DeleteGolferEndpointTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task DeleteGolfer_WhenGolferIsDeleted_ShouldReturnNoContent()
    {
        // Arrange
        using var client = Mother.CreateAuthorizedClient(golfApiFactory, isAdmin: true, isTrusted: false);
        var createdGolfer = await Mother.CreateGolferAsync(client);

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{createdGolfer!.GolferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteGolfer_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = golfApiFactory.CreateClient();
        int golferId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{golferId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, true, false)]
    [InlineData(HttpStatusCode.Forbidden, false, false)]
    [InlineData(HttpStatusCode.Forbidden, false, true)]
    public async Task DeleteGolfer_WhenGolferDoesNotExistOrUnauthorized_ShouldReturnExpectedStatusCode(
        HttpStatusCode expectedStatusCode,
        bool isAdmin,
        bool isTrusted)
    {
        // Arrange
        using var client = Mother
            .CreateAuthorizedClient(golfApiFactory, isTrusted: isTrusted, isAdmin: isAdmin);
        int golferId = Mother.GeneratePositiveInteger();

        // Act
        var response = await client.DeleteAsync($"{Mother.GolfersApiBasePath}/{golferId}");

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
    }
}