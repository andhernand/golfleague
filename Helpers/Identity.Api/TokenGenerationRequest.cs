namespace Identity.Api;

public class TokenGenerationRequest
{
    public required bool IsAdmin { get; init; }
    public required bool IsTrusted { get; init; }
}