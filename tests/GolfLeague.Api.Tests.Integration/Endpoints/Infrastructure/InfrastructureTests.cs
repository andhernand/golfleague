using System.Net;
using System.Net.Http.Headers;

using FluentAssertions;

namespace GolfLeague.Api.Tests.Integration.Endpoints.Infrastructure;

public class InfrastructureTests(GolfApiFactory golfApiFactory) : IClassFixture<GolfApiFactory>
{
    [Fact]
    public async Task Authentication_WhenJwtTokenIsExpired_ShouldFailWithUnauthenticated()
    {
        // arrange
        using var client = golfApiFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", JwtGenerator.GenerateToken(customTimeSpan: TimeSpan.FromSeconds(1)));

        await Task.Delay(TimeSpan.FromSeconds(7)); // make sure the ClockSkew (5 seconds) has passed.

        // act
        var response = await client.GetAsync(Mother.GolfersApiBasePath);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_WhenHealthy_ShouldReturnHealthy()
    {
        using var client = golfApiFactory.CreateClient();

        var response = await client.GetAsync("/_health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("Healthy");
    }
}