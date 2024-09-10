namespace Identity.Api.Requests;

public record TokenGenerationRequest
{
    public required bool IsAdmin { get; init; }
    public required bool IsTrusted { get; init; }
}