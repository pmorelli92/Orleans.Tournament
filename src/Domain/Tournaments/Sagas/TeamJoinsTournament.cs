using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Abstractions.Grains;
using Orleans.Tournament.Domain;
using Orleans.Tournament.Domain.Teams;
using Orleans.Tournament.Domain.Teams.Commands;
using Orleans.Tournament.Domain.Tournaments.Events;

namespace Orleans.Tournament.Domain.Tournaments.Sagas
{
    [ImplicitStreamSubscription(Constants.StreamNamespace)]
    public class TeamJoinsTournament : SubscriberGrain
    {
        public TeamJoinsTournament(ILogger<TeamJoinsTournament> logger)
            : base(
                new StreamOptions(Constants.TournamentStream, Constants.StreamNamespace),
                new PrefixLogger(logger, "[Tournament][Add Tournament To Team Saga]"))
        {
        }

        public override async Task HandleAsync(object evt, StreamSequenceToken token = null)
        {
            if (evt is TeamAdded obj)
            {
                var teamGrain = GrainFactory.GetGrain<ITeamGrain>(obj.TeamId);
                await teamGrain.JoinTournamentAsync(
                    new JoinTournament(obj.TeamId, obj.TournamentId, obj.TraceId, obj.InvokerUserId));
            }
        }
    }
}
