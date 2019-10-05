namespace Orleans.Tournament.Domain.Teams
open Microsoft.Extensions.Logging
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Abstractions
open Orleans.Tournament.Domain.Abstractions.Grains
open System.Threading.Tasks
open Orleans

type ITeamGrain =
    inherit IGrainWithGuidKey
    // Commands
    abstract CreateAsync: CreateTeam -> Task
    abstract AddPlayerAsync: AddPlayer -> Task
    abstract JoinTournamentAsync: JoinTournament -> Task
    // Queries
    abstract TeamExistAsync: unit -> Task<bool>

type TeamGrain(logger : ILogger<TeamGrain>) =
    inherit EventSourcedGrain<TeamState>(
        new StreamOptions(TeamStream, StreamNamespace),
        new PrefixLogger(logger, "[Team][Grain]"))

    interface ITeamGrain with
        member x.CreateAsync cmd =
            match Rules.TeamDoesNotExists x.State with
            | Ok _ -> x.PersistPublishAsync (TeamCreated.From cmd)
            | Error error -> x.EmitErrorAsync error cmd

        member x.AddPlayerAsync cmd =
            match Rules.TeamExists x.State with
            | Ok _ -> x.PersistPublishAsync (PlayerAdded.From cmd)
            | Error error -> x.EmitErrorAsync error cmd

        // Saga command
        member x.JoinTournamentAsync cmd =
            x.PersistPublishAsync (TournamentJoined.From cmd)

        member x.TeamExistAsync() =
            Task.FromResult(x.State.Created)

    member x.EmitErrorAsync (error : BusinessErrors) (cmd : ITraceable) =
        base.PublishErrorAsync((int error), error.ToString(), cmd.TraceId, cmd.InvokerUserId)

