namespace Orleans.Tournament.Domain.Tournaments
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Abstractions;
open Orleans.Tournament.Domain.Abstractions.Grains;
open Orleans.Streams;
open Microsoft.Extensions.Logging;
open Orleans.Tournament.Domain.Teams
open System.Threading.Tasks;
open Orleans;

[<ImplicitStreamSubscription(TournamentNamespace)>]
type TeamJoinsTournament(logger : ILogger<TeamJoinsTournament>) =
    inherit SubscriberGrain(
        new StreamOptions(MemoryProvider, TournamentNamespace),
        logger)
    override x.HandleAsync(evt : obj, token : StreamSequenceToken) =
        match evt with
        | :? TeamAdded as ta ->
            let teamGrain = x.GrainFactory.GetGrain<ITeamGrain>(ta.TeamId)
            let joinTournamentCmd = { JoinTournament.TeamId = ta.TeamId;
                                      TournamentId = ta.TournamentId;
                                      TraceId = ta.TraceId;
                                      InvokerUserId = ta.InvokerUserId }
            teamGrain.JoinTournamentAsync joinTournamentCmd |> ignore
            Task.FromResult(true)
        | _ -> Task.FromResult(false)

