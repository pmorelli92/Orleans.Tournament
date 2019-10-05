namespace Orleans.Tournament.Domain.Tournaments
open Microsoft.Extensions.Logging
open Orleans.Tournament.Domain.Abstractions
open Orleans.Tournament.Domain.Abstractions.Grains
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Teams
open System.Threading.Tasks
open Orleans

type ITournamentGrain =
    inherit IGrainWithGuidKey
    // Commands
    abstract CreateAsync: CreateTournament -> Task
    abstract AddTeamAsync: AddTeam -> Task
    abstract StartAsync: StartTournament -> Task
    abstract SetMatchResultAsync: SetMatchResult -> Task

type TournamentGrain(logger : ILogger<TournamentGrain>) =
    inherit EventSourcedGrain<TournamentState>(
        new StreamOptions(TournamentStream, StreamNamespace),
        new PrefixLogger(logger, "[Tournament][Grain]"))

    interface ITournamentGrain with
        member x.CreateAsync cmd =
            match Rules.TournamentDoesNotExists x.State with
            | Ok _ -> x.PersistPublishAsync (TournamentCreated.From cmd)
            | Error error -> x.EmitErrorAsync error cmd

        member x.AddTeamAsync cmd =
            // There is no problem on validating on the controller using the
            // read side tables, but this validation should be done on the domain
            // as this is the entry point of truth
            let teamGrain = x.GrainFactory.GetGrain<ITeamGrain>(cmd.TeamId);
            let teamExist = teamGrain.TeamExistAsync() |> Async.AwaitTask |> Async.RunSynchronously

            match
                Rules.TournamentExists(x.State) <&>
                Rules.TeamExistsForward(teamExist) <&>
                Rules.TeamIsNotAdded x.State cmd.TeamId <&>
                Rules.LessThanEightTeams x.State with
              | Ok _ -> x.PersistPublishAsync (TeamAdded.From cmd)
              | Error error -> x.EmitErrorAsync error cmd

        member x.StartAsync cmd =
            Task.CompletedTask

        member x.SetMatchResultAsync cmd =
            Task.CompletedTask;

    member x.EmitErrorAsync (error : BusinessErrors) (cmd : ITraceable) =
        base.PublishErrorAsync((int error), error.ToString(), cmd.TraceId, cmd.InvokerUserId)

