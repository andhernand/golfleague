using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Application.Mapping;

public static class ContractMapping
{
    public static GolferResponse MapToResponse(this Golfer golfer)
    {
        return new GolferResponse
        {
            GolferId = golfer.GolferId,
            FirstName = golfer.FirstName,
            LastName = golfer.LastName,
            Email = golfer.Email,
            JoinDate = golfer.JoinDate,
            Handicap = golfer.Handicap,
            Tournaments = golfer.Tournaments.Select(MapToResponse)
        };
    }

    public static IEnumerable<GolferResponse> MapToResponse(this IEnumerable<Golfer> golfers)
    {
        return golfers.Select(MapToResponse);
    }

    public static Golfer MapToGolfer(this CreateGolferRequest request)
    {
        return new Golfer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            JoinDate = request.JoinDate,
            Handicap = request.Handicap,
            Tournaments = []
        };
    }

    public static Golfer MapToGolfer(this UpdateGolferRequest request)
    {
        return new Golfer
        {
            GolferId = request.GolferId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            JoinDate = request.JoinDate,
            Handicap = request.Handicap,
            Tournaments = []
        };
    }

    public static TournamentResponse MapToResponse(this Tournament tournament)
    {
        return new TournamentResponse
        {
            TournamentId = tournament.TournamentId,
            Name = tournament.Name,
            Format = tournament.Format,
            Participants = tournament.Participants.Select(MapToResponse)
        };
    }

    public static IEnumerable<TournamentResponse> MapToResponse(this IEnumerable<Tournament> tournaments)
    {
        return tournaments.Select(MapToResponse);
    }

    public static Tournament MapToTournament(this CreateTournamentRequest request)
    {
        return new Tournament { Name = request.Name, Format = request.Format, Participants = [] };
    }

    public static Tournament MapToTournament(this UpdateTournamentRequest request, int tournamentId)
    {
        return new Tournament
        {
            TournamentId = tournamentId, Name = request.Name, Format = request.Format, Participants = []
        };
    }

    public static TournamentDetailResponse MapToResponse(this TournamentDetail tournamentDetail)
    {
        return new TournamentDetailResponse
        {
            TournamentId = tournamentDetail.TournamentId,
            Name = tournamentDetail.Name,
            Format = tournamentDetail.Format,
            Year = tournamentDetail.Year,
            Score = tournamentDetail.Score
        };
    }

    public static ParticipationDetailResponse MapToResponse(this ParticipationDetail participationDetail)
    {
        return new ParticipationDetailResponse
        {
            GolferId = participationDetail.GolferId,
            FirstName = participationDetail.FirstName,
            LastName = participationDetail.LastName,
            Year = participationDetail.Year,
            Score = participationDetail.Score
        };
    }

    public static TournamentParticipation MapToTournamentParticipation(
        this CreateParticipationDetailRequest detailRequest,
        int golferId)
    {
        return new TournamentParticipation
        {
            GolferId = golferId,
            TournamentId = detailRequest.TournamentId,
            Year = detailRequest.Year,
            Score = detailRequest.Score
        };
    }

    public static TournamentParticipation MapToTournamentParticipation(
        this CreateTournamentDetailRequest request,
        int tournamentId)
    {
        return new TournamentParticipation
        {
            GolferId = request.GolferId, TournamentId = tournamentId, Year = request.Year, Score = request.Score
        };
    }

    public static TournamentParticipationResponse MapToResponse(this TournamentParticipation participation)
    {
        return new TournamentParticipationResponse
        {
            GolferId = participation.GolferId,
            TournamentId = participation.TournamentId,
            Year = participation.Year,
            Score = participation.Score
        };
    }
}