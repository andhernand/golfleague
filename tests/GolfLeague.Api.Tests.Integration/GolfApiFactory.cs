using System.Data;

using Dapper;

using DotNetEnv;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

namespace GolfLeague.Api.Tests.Integration;

public class GolfApiFactory : WebApplicationFactory<IGolfApiMarker>, IAsyncLifetime
{
    private static readonly string ApiUserName;
    private static readonly string ApiUserPassword;
    private static readonly string SaPassword;
    private static readonly string SolutionFolder;

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithPassword(SaPassword)
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("MSSQL_PID", "Developer")
        .Build();

    static GolfApiFactory()
    {
        Env.TraversePath().Load();

        ApiUserName = GetEnvironmentVariableOrThrow(VariableNames.GolfApiUserName);
        ApiUserPassword = GetEnvironmentVariableOrThrow(VariableNames.GolfApiUserPassword);
        SaPassword = GetEnvironmentVariableOrThrow(VariableNames.GolfApiSaPassword);
        SolutionFolder = DirectoryFinder.GetDirectoryContaining(VariableNames.SolutionFileName)
                         ?? throw new DirectoryNotFoundException("Solution folder not found");
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync().ConfigureAwait(false);
        await InitializeDatabaseAsync().ConfigureAwait(false);
        await RunDatabaseMigrationScriptsAsync().ConfigureAwait(false);
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync().ConfigureAwait(false);
        await _sqlContainer.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:GolfLeagueDb", BuildDatabaseUserConnectionString());
        base.ConfigureWebHost(builder);
    }

    private async Task InitializeDatabaseAsync()
    {
        var initScriptPath = Path.Combine(SolutionFolder, "db", "init.sql");

        if (!File.Exists(initScriptPath))
        {
            throw new FileNotFoundException($"{initScriptPath} file not found");
        }

        var initScript = await File.ReadAllTextAsync(initScriptPath).ConfigureAwait(false);
        var script = initScript.Replace("$(varApiPassword)", ApiUserPassword);
        await _sqlContainer.ExecScriptAsync(script).ConfigureAwait(false);
    }

    private async Task RunDatabaseMigrationScriptsAsync()
    {
        var migrationScripts = LoadMigrationScriptsFromFileSystem();
        await using var connection = new SqlConnection(BuildDatabaseUserConnectionString("sa", SaPassword));
        await connection.OpenAsync().ConfigureAwait(false);

        foreach (var migrationScript in migrationScripts)
        {
            var script = await File.ReadAllTextAsync(migrationScript).ConfigureAwait(false);
            var migrations = script.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);

            foreach (var migration in migrations)
            {
                await connection.ExecuteAsync(
                        new CommandDefinition(
                            migration,
                            commandType: CommandType.Text))
                    .ConfigureAwait(false);
            }
        }
    }

    private static IOrderedEnumerable<string> LoadMigrationScriptsFromFileSystem()
    {
        var migrationScriptsFolder = Path.Combine(SolutionFolder, "db", "migrations");

        if (!Directory.Exists(migrationScriptsFolder))
        {
            throw new DirectoryNotFoundException($"Migration scripts folder not found: {migrationScriptsFolder}");
        }

        return Directory.GetFiles(migrationScriptsFolder, "V*.sql").OrderBy(f => f);
    }

    private string BuildDatabaseUserConnectionString(string? userName = null, string? password = null)
    {
        return new SqlConnectionStringBuilder
        {
            DataSource = $"{_sqlContainer.Hostname},{_sqlContainer.GetMappedPublicPort(MsSqlBuilder.MsSqlPort)}",
            InitialCatalog = VariableNames.GolfApiDatabaseName,
            UserID = userName ?? ApiUserName,
            Password = password ?? ApiUserPassword,
            ApplicationName = VariableNames.GolfLeagueApiApplicationName,
            ApplicationIntent = ApplicationIntent.ReadWrite,
            TrustServerCertificate = true
        }.ConnectionString;
    }

    private static string GetEnvironmentVariableOrThrow(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName)
               ?? throw new InvalidOperationException($"Environment variable not set: {variableName}");
    }

    private static class VariableNames
    {
        public const string GolfApiUserPassword = "GOLF_API_USER_PASSWORD";
        public const string GolfApiUserName = "GOLF_API_USER_NAME";
        public const string GolfApiSaPassword = "GOLF_API_SA_PASSWORD";
        public const string SolutionFileName = "GolfLeague.sln";
        public const string GolfApiDatabaseName = "GolfLeague";
        public const string GolfLeagueApiApplicationName = "GolfLeagueApiTests";
    }
}