namespace Orleans.Tournament.Domain.Tournaments
open System
open Orleans.Tournament.Domain.Abstractions

type CreateTournament =
    { Name:string
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

type AddTeam =
    { TournamentId:Guid
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
        interface ITraceable with
          member x.TraceId = x.TraceId
          member x.InvokerUserId = x.InvokerUserId

type StartTournament =
    { TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
        interface ITraceable with
          member x.TraceId = x.TraceId
          member x.InvokerUserId = x.InvokerUserId

type SetMatchResult =
    { TournamentId:Guid
      MatchInfo:MatchInfo
      TraceId:Guid
      InvokerUserId:Guid }
        interface ITraceable with
          member x.TraceId = x.TraceId
          member x.InvokerUserId = x.InvokerUserId