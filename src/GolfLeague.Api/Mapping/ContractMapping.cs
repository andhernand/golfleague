using GolfLeague.Application.Models;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Mapping;

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
            Handicap = request.Handicap,
            Tournaments = []
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

    public static TournamentsResponse MapToResponse(this IEnumerable<Tournament> tournaments)
    {
        return new TournamentsResponse { Tournaments = tournaments.Select(MapToResponse) };
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

    public static TournamentParticipationResponse MapToResponse(this TournamentParticipation tournamentParticipation)
    {
        return new TournamentParticipationResponse
        {
            GolferId = tournamentParticipation.GolferId,
            TournamentId = tournamentParticipation.TournamentId,
            Year = tournamentParticipation.Year,
            Score = tournamentParticipation.Score
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
        this CreateGolferTournamentParticipationRequest request,
        int golferId)
    {
        return new TournamentParticipation
        {
            GolferId = golferId, TournamentId = request.TournamentId, Year = request.Year, Score = request.Score
        };
    }

    public static TournamentParticipation MapToTournamentParticipation(
        this CreateTournamentGolferParticipationRequest request,
        int tournamentId)
    {
        return new TournamentParticipation
        {
            GolferId = request.GolferId, TournamentId = tournamentId, Year = request.Year, Score = request.Score
        };
    }

    public static UpdateTournamentParticipation MapToUpdateTournamentParticipation(
        this UpdateGolferTournamentParticipationRequest request,
        int golferId)
    {
        return new UpdateTournamentParticipation
        {
            Original = new TournamentParticipation
            {
                GolferId = golferId, TournamentId = request.OriginalTournamentId, Year = request.OriginalYear
            },
            Update = new TournamentParticipation
            {
                GolferId = golferId,
                TournamentId = request.NewTournamentId,
                Year = request.NewYear,
                Score = request.NewScore
            }
        };
    }

    public static UpdateTournamentParticipation MapToUpdateTournamentParticipation(
        this UpdateTournamentGolferParticipationRequest request,
        int tournamentId)
    {
        return new UpdateTournamentParticipation
        {
            Original = new TournamentParticipation
            {
                GolferId = request.OriginalGolferId, TournamentId = tournamentId, Year = request.OriginalYear
            },
            Update = new TournamentParticipation
            {
                GolferId = request.NewGolferId,
                TournamentId = tournamentId,
                Year = request.NewYear,
                Score = request.NewScore
            }
        };
    }
}