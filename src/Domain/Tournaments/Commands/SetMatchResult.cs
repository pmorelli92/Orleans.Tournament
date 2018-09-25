using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Tournaments.ValueObject;

namespace Snaelro.Domain.Tournaments.Commands
{
    public class SetMatchResult : ITraceable
    {
        public Guid TournamentId { get; }

        public MatchSummary MatchSummary { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public SetMatchResult(Guid tournamentId, MatchSummary matchSummary, Guid traceId, Guid invokerUserId)
        {
            TournamentId = tournamentId;
            MatchSummary = matchSummary;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}