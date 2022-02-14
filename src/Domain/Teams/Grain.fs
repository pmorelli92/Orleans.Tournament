namespace Orleans.Tournament.Domain.Teams
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Abstractions
open Orleans.Tournament.Domain.Abstractions.Grains
open System.Threading.Tasks
open Orleans
open Orleans.Tournament.Domain.Abstractions.Events

type ITeamGrain =
    inherit IGrainWithGuidKey
    // Commands
    abstract CreateAsync: CreateTeam -> Task
    abstract AddPlayerAsync: AddPlayer -> Task
    abstract JoinTournamentAsync: JoinTournament -> Task
    // Queries
    abstract TeamExistAsync: unit -> Task<bool>

type TeamGrain() =
    inherit EventSourcedGrain<TeamState>(
        new StreamConfig(InMemoryStream, TeamNamespace))

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
            // The way below also works an it is more fsharp idiomatic
            // let wrap value = async { return value } |> Async.StartAsTask
            // wrap x.State.Created

    member x.EmitErrorAsync (error : BusinessErrors) (cmd : ITraceable) =
        base.PublishErrorAsync(new ErrorHasOccurred((int error), error.ToString(), cmd.TraceId, cmd.InvokerUserId))
