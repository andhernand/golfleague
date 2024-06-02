# Golf League

This project is a minimal API built with .NET 8.0, using Dapper for database access, Docker Desktop to run an MSSQL server, FluentValidation for input validation, and Testcontainers for integration testing.

Plans exist to add a web UI for integrating with the minimal API.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/andhernand/golfleague.git
cd golfleague
```

### Setup Environment Variables

The [.env](.env) file in the root directory of the project contains the environment variables necessary for running the application.

***NOTE:*** In the [appsettings.json](src/GolfLeague.Api/appsettings.json) file, the ConnectionString contains hard-coded values that are not synced with the [.env](.env) file. If you make changes to the [.env](.env) file and don't update the [appsettings.json](src/GolfLeague.Api/appsettings.json) file, you may run into problems.

***NOTE:*** I chose to store the passwords in the .env file, for example only. Please use secrets or a key/value vault.

### Start MSSQL Server with Docker

Make sure Docker Desktop is running, then execute the following command to start the MSSQL server container:

[Flyway](https://flywaydb.org/) by [Redgate](https://www.red-gate.com/) is used to manage and execute database migrations in this project.

```bash
docker-compose up -d
```

Take a look at the [docker-compose.yml](docker-compose.yml) file in the root directory of the project for configuration details.

### Restore and Run the API

```bash
dotnet build
dotnet run
```

The API will be available at [https://localhost:7184](https://localhost:7184). You can view the OpenAPI Document at [https://localhost:7184/swagger](https://localhost:7184/swagger).

***NOTE:*** This can be changed in the [launchSettings.json](src/GolfLeague.Api/Properties/launchSettings.json) file.

### Authentication and Authorization

The API is set up to only accept authenticated users. I have provided an additional [Identity.Api](Helpers/Identity.Api) project for generating JWT Tokens. There is a [README](Helpers/README.MD) file to explain how to use it.

* An Authenticated user can only call HTTP GET methods (read-only).
* A Trusted user can call HTTP GET, HTTP POST, and HTTP PUT methods (read-write).
* An Admin user can call HTTP GET, HTTP POST, HTTP PUT, and HTTP DELETE methods (read-write-delete).

### Database Access with Dapper

This project uses Dapper for database access. You can find the database context and repository implementations in the [Database](src/GolfLeague.Application/Database) folder.

### Input Validation with FluentValidation

FluentValidation is used to handle input validation. Validators are located in the [Validators](src/GolfLeague.Application/Validators) folder.

### Integration Testing with Testcontainers

Testcontainers is used to create MSSQL databases for integration tests. Tests are located in the [Tests](tests) folder. folder. To run the tests, use the following command:

```bash
dotnet test
```

### Contributing

Contributions are welcome! Please fork this repository and submit a pull request with your changes.

### License

This project is licensed under the MIT License. See the [LICENSE](License) file for more details.
