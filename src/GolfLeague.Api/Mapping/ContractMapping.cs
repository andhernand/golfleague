using GolfLeague.Application.Models;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Mapping;

public static class ContractMapping
{
    public static MemberResponse MapToResponse(this Member member)
    {
        return new MemberResponse
        {
            MemberId = member.MemberId,
            FirstName = member.FirstName,
            LastName = member.LastName,
            MemberTypeId = member.MemberTypeId,
            JoinDate = member.JoinDate,
            Handicap = member.Handicap,
            Phone = member.Phone,
            Coach = member.Coach,
            Gender = member.Gender
        };
    }

    public static MembersResponse MapToResponse(this IEnumerable<Member> members)
    {
        return new MembersResponse { Members = members.Select(MapToResponse) };
    }
}