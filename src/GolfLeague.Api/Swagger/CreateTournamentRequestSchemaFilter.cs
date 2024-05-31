using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace GolfLeague.Api.Swagger;

// ReSharper disable once ClassNeverInstantiated.Global
public class CreateTournamentRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(CreateTournamentRequest))
        {
            return;
        }

        var propertyName = nameof(CreateTournamentRequest.Format).ToLowerInvariant();
        if (schema.Properties.TryGetValue(propertyName, out OpenApiSchema? schemaProperty))
        {
            schemaProperty.Enum = TournamentFormat.Values
                .Select(v => new OpenApiString(v))
                .Cast<IOpenApiAny>()
                .ToList();
        }
    }
}