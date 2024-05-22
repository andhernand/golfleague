using Bogus;

using GolfLeague.Contracts.Requests;

namespace GolfLeague.Api.Tests.Integration;

public static class Fakers
{
    public static CreateGolferRequest GenerateCreateGolferRequest(
        string? firstName = default,
        string? lastName = default,
        string? email = default,
        DateTime? joinDate = default
    )
    {
        return new Faker<CreateGolferRequest>()
            .RuleFor(r => r.FirstName, f => firstName ?? f.Name.FirstName())
            .RuleFor(r => r.LastName, f => lastName ?? f.Name.LastName())
            .RuleFor(r => r.Email, f => email ?? f.Person.Email)
            .RuleFor(r => r.JoinDate, f => joinDate ?? f.Date.Past(4))
            .RuleFor(r => r.Handicap, f => f.Random.Int(0, 54).OrNull(f))
            .Generate();
    }

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