using Microsoft.AspNetCore.Mvc.Testing;

namespace GolfLeague.Api.Tests.Integration;

public class GolfApiFactory : WebApplicationFactory<IGolfApiMarker>
{
    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     // This is how you use a different connection string during test execution.
    //     builder.UseSetting("Database:ConnectionString", "differentConnectionString");
    //     base.ConfigureWebHost(builder);
    // }
}