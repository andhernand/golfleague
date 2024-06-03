using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace GolfLeague.Api.Swagger;

public class UpdateTournamentRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(UpdateTournamentRequest))
        {
            return;
        }

        var propertyName = nameof(UpdateTournamentRequest.Format).ToLowerInvariant();
        if (schema.Properties.TryGetValue(propertyName, out OpenApiSchema? schemaProperty))
        {
            schemaProperty.Enum = TournamentFormat.Values
                .Select(v => new OpenApiString(v))
                .Cast<IOpenApiAny>()
                .ToList();
        }
    }
}