namespace Orleans.Tournament.Domain.Tournaments
open System
open Orleans.Tournament.Domain.Abstractions

type TournamentCreated =
    { Name:string
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
      member x.TraceId = x.TraceId
      member x.InvokerUserId = x.InvokerUserId
    static member From (cmd : CreateTournament) =
      { Name = cmd.Name;
        TournamentId = cmd.TournamentId;
        TraceId = cmd.TraceId;
        InvokerUserId = cmd.InvokerUserId; }

type TeamAdded =
    { TeamId:Guid
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
      member x.TraceId = x.TraceId
      member x.InvokerUserId = x.InvokerUserId
    static member From (cmd : AddTeam) =
      { TeamId = cmd.TeamId;
        TournamentId = cmd.TournamentId;
        TraceId = cmd.TraceId;
        InvokerUserId = cmd.InvokerUserId; }

type TournamentStarted =
    { TournamentId:Guid
      Teams:list<Guid>
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
      member x.TraceId = x.TraceId
      member x.InvokerUserId = x.InvokerUserId
    static member From (cmd : StartTournament) teams =
      { TournamentId = cmd.TournamentId;
        Teams = teams
        TraceId = cmd.TraceId;
        InvokerUserId = cmd.InvokerUserId; }

type MatchResultSet =
    { TournamentId:Guid
      MatchInfo:MatchInfo
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
      member x.TraceId = x.TraceId
      member x.InvokerUserId = x.InvokerUserId
    static member From (cmd : SetMatchResult) =
      { TournamentId = cmd.TournamentId;
        MatchInfo = cmd.MatchInfo;
        TraceId = cmd.TraceId;
        InvokerUserId = cmd.InvokerUserId; }
