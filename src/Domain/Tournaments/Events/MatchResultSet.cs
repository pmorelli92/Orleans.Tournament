using System;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Tournaments.Commands;
using Snaelro.Domain.Tournaments.ValueObject;

namespace Snaelro.Domain.Tournaments.Events
{
    public class MatchResultSet : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public MatchResult MatchResult { get; }

        public MatchResultSet(Guid tournamentId, Guid traceId, Guid invokerUserId, MatchResult matchResult)
        {
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
            MatchResult = matchResult;
        }

        public static MatchResultSet From(SetMatchResult cmd)
            => new MatchResultSet(cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId, cmd.MatchResult);
    }
}