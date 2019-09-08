
namespace Orleans.Tournament.Domain.Teams.Events
open System
open Orleans.Tournament.Domain.Abstractions
open Orleans.Tournament.Domain.Teams.Commands

type PlayerAdded =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (addPlayer : AddPlayer) =
    { Name = addPlayer.Name;
      TeamId = addPlayer.TeamId;
      TraceId = addPlayer.TraceId;
      InvokerUserId = addPlayer.TraceId }

type TeamCreated =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (createTeam : CreateTeam) =
        { Name = createTeam.Name;
          TeamId = createTeam.TeamId;
          TraceId = createTeam.TraceId;
          InvokerUserId = createTeam.TraceId }

type TournamentJoined =
    { TeamId:Guid
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

    static member From (joinTournament : JoinTournament) =
    { TeamId = joinTournament.TeamId;
      TournamentId = joinTournament.TournamentId;
      TraceId = joinTournament.TraceId;
      InvokerUserId = joinTournament.TraceId }
