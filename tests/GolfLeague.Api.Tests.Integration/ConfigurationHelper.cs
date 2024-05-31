namespace GolfLeague.Api.Tests.Integration;

public static class ConfigurationHelper
{
    public static IConfiguration Configuration { get; }

    static ConfigurationHelper()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: false)
            .Build();
    }
}