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
            MemberTypeId = memberType.MemberTypeId, Name = memberType.Name, Fee = memberType.Fee
        };
    }

    public static MemberTypesResponse MapToResponse(this IEnumerable<MemberType> memberTypeResponses)
    {
        return new MemberTypesResponse { MemberTypes = memberTypeResponses.Select(MapToResponse) };
    }

    public static MemberType MapToMemberType(this CreateMemberTypeRequest request)
    {
        return new MemberType { Name = request.Name, Fee = request.Fee };
    }

    public static MemberType MapToMemberType(this UpdateMemberTypeRequest request, int id)
    {
        return new MemberType { MemberTypeId = id, Name = request.Name, Fee = request.Fee };
    }

    public static GolferResponse MapToResponse(this Golfer golfer)
    {
        return new GolferResponse
        {
            GolferId = golfer.GolferId,
            FirstName = golfer.FirstName,
            LastName = golfer.LastName,
            Email = golfer.Email,
            JoinDate = golfer.JoinDate,
            Handicap = golfer.Handicap
        };
    }

    public static GolfersResponse MapToResponse(this IEnumerable<Golfer> golfers)
    {
        return new GolfersResponse { Golfers = golfers.Select(MapToResponse) };
    }

    public static Golfer MapToGolfer(this CreateGolferRequest request)
    {
        return new Golfer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            JoinDate = request.JoinDate,
            Handicap = request.Handicap
        };
    }

    public static Golfer MapToGolfer(this UpdateGolferRequest request, int id)
    {
        return new Golfer
        {
            GolferId = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            JoinDate = request.JoinDate,
            Handicap = request.Handicap
        };
    }
}