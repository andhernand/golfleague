using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Options;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";
    [Required] public required string Key { get; init; }
    [Required] public required string Issuer { get; init; }
    [Required] public required string Audience { get; init; }
}