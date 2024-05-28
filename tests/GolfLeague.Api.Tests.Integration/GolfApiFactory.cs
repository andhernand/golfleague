using System.Data;

using Dapper;

using DotNetEnv;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

namespace GolfLeague.Api.Tests.Integration;

// ReSharper disable once ClassNeverInstantiated.Global
public class GolfApiFactory : WebApplicationFactory<IGolfApiMarker>, IAsyncLifetime
{
    private const string GolfApiDatabaseName = "GolfLeague";
    private const string GolfLeagueApiApplicationName = "GolfLeagueApiTests";

    private readonly string? _golfApiUserName;
    private readonly string? _golfApiUserPassword;
    private readonly string? _saPassword;
    private readonly string _solutionFolder;
    private readonly MsSqlContainer _sqlContainer;

    public GolfApiFactory()
    {
        Env.TraversePath().Load();
        _saPassword = Environment.GetEnvironmentVariable("GOLF_API_SA_PASSWORD");
        _golfApiUserPassword = Environment.GetEnvironmentVariable("GOLF_API_USER_PASSWORD");
        _golfApiUserName = Environment.GetEnvironmentVariable("GOLF_API_USER_NAME");

        _solutionFolder = DirectoryFinder.GetDirectoryContaining("GolfLeague.sln");

        _sqlContainer = new MsSqlBuilder()
            .WithPassword(_saPassword)
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("MSSQL_PID", "Developer")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync().ConfigureAwait(false);
        await InitializeDatabaseAync().ConfigureAwait(false);
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
        builder.UseSetting("Database:ConnectionString", BuildDatabaseUserConnectionString());
        base.ConfigureWebHost(builder);
    }

    private async Task InitializeDatabaseAync()
    {
        var initScriptPath = Path.Combine(_solutionFolder, "db", "init.sql");

        if (!Path.Exists(initScriptPath))
        {
            throw new FileNotFoundException($"{initScriptPath} file not found");
        }

        var initScript = await File.ReadAllTextAsync(initScriptPath).ConfigureAwait(false);
        var script = initScript.Replace("$(varApiPassword)", _golfApiUserPassword);
        await _sqlContainer.ExecScriptAsync(script).ConfigureAwait(false);
    }

    private async Task RunDatabaseMigrationScriptsAsync()
    {
        var scriptFiles = LoadMigrationScriptsFromFileSystem();
        await using var connection = new SqlConnection(BuildDatabaseUserConnectionString("sa", _saPassword));
        await connection.OpenAsync().ConfigureAwait(false);

        foreach (var scriptFile in scriptFiles)
        {
            var script = await File.ReadAllTextAsync(scriptFile).ConfigureAwait(false);
            var commands = script.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);

            foreach (var command in commands)
            {
                try
                {
                    await connection.ExecuteAsync(
                            new CommandDefinition(
                                command,
                                commandType: CommandType.Text))
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {command}. Exception: {ex}");
                    throw;
                }
            }
        }
    }

    private IOrderedEnumerable<string> LoadMigrationScriptsFromFileSystem()
    {
        var migrationScriptsFolder = Path.Combine(_solutionFolder, "db", "migrations");

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
            InitialCatalog = GolfApiDatabaseName,
            UserID = userName ?? _golfApiUserName,
            Password = password ?? _golfApiUserPassword,
            ApplicationName = GolfLeagueApiApplicationName,
            ApplicationIntent = ApplicationIntent.ReadWrite,
            TrustServerCertificate = true
        }.ConnectionString;
    }
}