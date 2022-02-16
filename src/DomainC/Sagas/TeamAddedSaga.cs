using Orleans;
using Orleans.Streams;
using Tournament.Domain.Abstractions;
using Tournament.Domain.Abstractions.Grains;
using Tournament.Domain.Teams;
using Tournament.Domain.Tournaments;

namespace Tournament.Domain.Sagas;

// Subscribes to the InMemoryStream for the TournamentNamespace
// For TeamAdded event, get the Team grain and publish the JoinTournament command
[ImplicitStreamSubscription(Constants.TournamentNamespace)]
public class TeamAddedSaga : SubscriberGrain
{
    public TeamAddedSaga()
        : base(new StreamConfig(Constants.InMemoryStream, Constants.TournamentNamespace))
    {
    }

    public override async Task<bool> HandleAsync(object evt, StreamSequenceToken token = null)
    {
        if (evt is not TeamAdded obj)
            return true;

        // We already know that the Team exists, as it was validated
        // on the AddTeam command, and before publishing the TeamAdded event
        var teamGrain = GrainFactory.GetGrain<ITeamGrain>(obj.TeamId);

        var joinTournamentCmd = new JoinTournament(obj.TeamId, obj.TournamentId, obj.TraceId, obj.InvokerUserId);
        await teamGrain.JoinTournamentAsync(joinTournamentCmd);
        return true;
    }
}