using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Mapping;

public static class ContractMapping
{
    public static MemberTypeResponse MapToResponse(this MemberType memberType)
    {
        return new MemberTypeResponse
        {
            MemberTypeId = memberType.MemberTypeId,
            Name = memberType.Name,
            Fee = memberType.Fee
        };
    }

    public static MemberTypesResponse MapToResponse(this IEnumerable<MemberType> memberTypeResponses)
    {
        return new MemberTypesResponse { MemberTypes = memberTypeResponses.Select(MapToResponse) };
    }

    public static MemberType MapToMemberType(this CreateMemberTypeRequest request)
    {
        return new MemberType
        {
            Name = request.Name,
            Fee = request.Fee
        };
    }

    public static MemberType MapToMemberType(this UpdateMemberTypeRequest request, int id)
    {
        return new MemberType
        {
            MemberTypeId = id,
            Name = request.Name, 
            Fee = request.Fee
        };
    }

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

    public static Member MapToMember(this CreateMemberRequest request)
    {
        return new Member
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            JoinDate = request.JoinDate,
            MemberTypeId = request.MemberTypeId,
            Handicap = request.Handicap,
            Phone = request.Phone,
            Coach = request.Coach
        };
    }
}