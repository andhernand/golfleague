// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Identity.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class TokenGenerationRequest
{
    public required bool IsAdmin { get; init; }
    public required bool IsTrusted { get; init; }
}