namespace Orleans.Tournament.Domain.Teams
open System
open Orleans.Tournament.Domain.Abstractions

type PlayerAdded =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (cmd : AddPlayer) =
        { Name = cmd.Name;
          TeamId = cmd.TeamId;
          TraceId = cmd.TraceId;
          InvokerUserId = cmd.TraceId }

type TeamCreated =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (cmd : CreateTeam) =
        { Name = cmd.Name;
          TeamId = cmd.TeamId;
          TraceId = cmd.TraceId;
          InvokerUserId = cmd.TraceId }

type TournamentJoined =
    { TeamId:Guid
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (cmd : JoinTournament) =
        { TeamId = cmd.TeamId;
          TournamentId = cmd.TournamentId;
          TraceId = cmd.TraceId;
          InvokerUserId = cmd.TraceId }
