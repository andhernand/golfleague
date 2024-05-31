using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace GolfLeague.Api.Swagger;

// ReSharper disable once ClassNeverInstantiated.Global
public class TournamentResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(TournamentResponse))
        {
            return;
        }

        var propertyName = nameof(TournamentResponse.Format).ToLowerInvariant();
        if (schema.Properties.TryGetValue(propertyName, out OpenApiSchema? schemaProperty))
        {
            schemaProperty.Enum = TournamentFormat.Values
                .Select(v => new OpenApiString(v))
                .Cast<IOpenApiAny>()
                .ToList();
        }
    }
}