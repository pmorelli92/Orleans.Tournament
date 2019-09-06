using System;
using Newtonsoft.Json;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Tournaments.Commands;

namespace Orleans.Tournament.Domain.Tournaments.Events
{
    public class TournamentCreated : ITraceable
    {
        public string Name { get; }

        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        [JsonConstructor]
        public TournamentCreated(string name, Guid tournamentId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

        public static TournamentCreated From(CreateTournament cmd)
            => new TournamentCreated(cmd.Name, cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId);
    }
}
