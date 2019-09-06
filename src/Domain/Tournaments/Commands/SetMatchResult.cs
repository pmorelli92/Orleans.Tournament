using System;
using Orleans.Tournament.Domain.Abstractions;
using Orleans.Tournament.Domain.Tournaments.ValueObject;

namespace Orleans.Tournament.Domain.Tournaments.Commands
{
    public class SetMatchResult : ITraceable
    {
        public Guid TournamentId { get; }

        public MatchResult MatchResult { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public SetMatchResult(Guid tournamentId, MatchResult matchResult, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            MatchResult = matchResult;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}
