namespace Orleans.Tournament.Domain.Teams
open System
open Orleans.Tournament.Domain.Abstractions

type AddPlayer =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

type CreateTeam =
    { Name:string
      TeamId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId

type JoinTournament =
    { TeamId:Guid
      TournamentId:Guid
      TraceId:Guid
      InvokerUserId:Guid }
    interface ITraceable with
        member x.TraceId = x.TraceId
        member x.InvokerUserId = x.InvokerUserId
