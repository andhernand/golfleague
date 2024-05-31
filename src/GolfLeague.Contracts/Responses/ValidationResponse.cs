// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace GolfLeague.Contracts.Responses;

public class ValidationResponse
{
    public required string PropertyName { get; init; }
    public required string ErrorMessage { get; init; }
}