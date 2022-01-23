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
        new StreamOptions(MemoryProvider, TournamentNamespace),
        logger)

    interface ITournamentGrain with
        member x.CreateAsync cmd =
            match Rules.TournamentDoesNotExists x.State with
            | Ok _ -> x.PersistPublishAsync (TournamentCreated.From cmd)
            | Error error -> x.EmitErrorAsync error cmd

        member x.AddTeamAsync cmd =
            // There is no problem on validating on the controller using the
            // read side tables, but this validation should be done on the domain
            // as this is the entry point of truth
            let teamGrain = x.GrainFactory.GetGrain<ITeamGrain>(cmd.TeamId)
            // TODO: Any other form of handling this, in an Fsharp way, deadlocks
            let task = Task.Run(fun _ -> teamGrain.TeamExistAsync().Result)

            match
                Rules.TournamentExists x.State <&>
                Rules.TeamExistsForward task.Result <&>
                Rules.TeamIsNotAdded x.State cmd.TeamId <&>
                Rules.LessThanEightTeams x.State with
              | Ok _ -> x.PersistPublishAsync (TeamAdded.From cmd)
              | Error error -> x.EmitErrorAsync error cmd

        member x.StartAsync cmd =
            match
                Rules.TournamentExists x.State <&>
                Rules.TournamentIsNotStarted x.State <&>
                Rules.EightTeamsToStartTournament x.State with
              | Ok _ -> x.PersistPublishAsync (TournamentStarted.From cmd x.State.Teams )
              | Error error -> x.EmitErrorAsync error cmd

        member x.SetMatchResultAsync cmd =
            match
                Rules.TournamentExists x.State <&>
                Rules.TournamentIsStarted x.State <&>
                Rules.MatchIsNotDraw cmd.MatchInfo.MatchSummary <&>
                Rules.MatchExistsAndIsNotPlayed x.State cmd.MatchInfo with
              | Ok _ -> x.PersistPublishAsync (MatchResultSet.From cmd)
              | Error error -> x.EmitErrorAsync error cmd

    member x.EmitErrorAsync (error : BusinessErrors) (cmd : ITraceable) =
        base.PublishErrorAsync((int error), error.ToString(), cmd.TraceId, cmd.InvokerUserId)

