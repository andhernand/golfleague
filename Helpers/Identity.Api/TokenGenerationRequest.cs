namespace Identity.Api;

public record TokenGenerationRequest
{
    public required bool IsAdmin { get; init; }
    public required bool IsTrusted { get; init; }
}