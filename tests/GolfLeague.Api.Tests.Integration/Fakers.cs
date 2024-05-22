using Bogus;

using GolfLeague.Contracts.Requests;

namespace GolfLeague.Api.Tests.Integration;

public static class Fakers
{
    public static CreateMemberTypeRequest GenerateCreateMemberTypeRequest(
        string? name = default,
        decimal? fee = default)
    {
        return new Faker<CreateMemberTypeRequest>()
            .RuleFor(request => request.Name, faker => name ?? faker.Hacker.Verb())
            .RuleFor(request => request.Fee, faker => fee ?? faker.Finance.Amount().OrNull(faker))
            .Generate();
    }

    public static UpdateMemberTypeRequest GenerateUpdateMemberTypeRequest(
        string? name = default,
        decimal? fee = default)
    {
        return new Faker<UpdateMemberTypeRequest>()
            .RuleFor(request => request.Name, faker => name ?? faker.Hacker.Verb())
            .RuleFor(request => request.Fee, faker => fee ?? faker.Finance.Amount().OrNull(faker))
            .Generate();
    }
}