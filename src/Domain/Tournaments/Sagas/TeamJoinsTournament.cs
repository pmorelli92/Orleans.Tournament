using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Abstractions.Grains;
using Snaelro.Domain.Snaelro.Domain;
using Snaelro.Domain.Teams;
using Snaelro.Domain.Teams.Commands;
using Snaelro.Domain.Tournaments.Events;

namespace Snaelro.Domain.Tournaments.Sagas
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