﻿# Identity API

A minimal API built with .NET 8.0, for generating JWT Tokens that can be used for Authentication/Authorization within the [Golf League API](../src/GolfLeague.Api).

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)

## Getting Started

### Setup Environment Variables

The [appsettings.Development.json](Identity.Api/appsettings.Development.json) file in the root directory of the project contains the **Key**, **Issuer**, and **Audience** key/value pairs. They are all necessary values for generating a valid JWT Token.

**_NOTE:_** The **Key**, **Issuer**, and **Audience** must match the values in the [GolfLeague.Api/appsettings.Development.json](../src/GolfLeague.Api/appsettings.Development.json) file. Failure to do so will result in unusable JWT Tokens being generated.s

```json
{
  "JwtSettings": {
    "Key": "YoHomeBoyStoreAndLoadThisSecurely",
    "Issuer": "https://id.andhernand.com",
    "Audience": "https://golfleague.andhernand.com/api"
  }
}
```

### Restore and Run the API

```bash
dotnet build
dotnet run
```

The API will be available at [https://localhost:7031](https://localhost:7031). There is only one HTTP POST endpoint [/api/tokens](https://localhost:7031/api/tokens). You can view the OpenAPI Document at [/swagger](https://localhost:7031/swagger) for information on how to call the endpoint.

* An Authenticated user can only call HTTP GET methods (read-only).
* A Trusted user can call HTTP GET, HTTP POST, and HTTP PUT methods (read-write).
* An Admin user can call HTTP GET, HTTP POST, HTTP PUT, and HTTP DELETE methods (read-write-delete).

### Examples

```http request
### Generate an Authenticated User
POST https://localhost:7031/api/tokens
Content-Type: application/json

{
  "isAdmin": false,
  "isTrusted": false
}

### Generate an Admin User
POST https://localhost:7031/api/tokens
Content-Type: application/json

{
  "isAdmin": true,
  "isTrusted": false
}

### Generate a Trusted User
POST https://localhost:7031/api/tokens
Content-Type: application/json

{
  "isAdmin": false,
  "isTrusted": true
}

###
```
