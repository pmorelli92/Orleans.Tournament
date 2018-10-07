using System;
using Newtonsoft.Json;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class MatchSummary
    {
        public Guid LocalTeamId { get; }

        public int? LocalGoals { get; }

        public Guid AwayTeamId { get; }

        public int? AwayGoals { get; }

        public bool Played { get; }

        [JsonConstructor]
        public MatchSummary(Guid localTeamId, Guid awayTeamId)
        {
            LocalTeamId = localTeamId;
            AwayTeamId = awayTeamId;
            Played = false;
        }

        public MatchSummary(Guid localTeamId, int localGoals, Guid awayTeamId, int awayGoals)
        {
            LocalTeamId = localTeamId;
            LocalGoals = localGoals;
            AwayTeamId = awayTeamId;
            AwayGoals = awayGoals;
            Played = true;
        }

        public MatchSummary SetResult(int localGoals, int awayGoals)
        {
            if (Played)
                throw new InvalidOperationException("The match is already played");

            return new MatchSummary(LocalTeamId, localGoals, AwayTeamId, awayGoals);
        }

        public bool ResultRelatesToMatch(MatchResult result)
            => LocalTeamId == result.LocalTeamId && AwayTeamId == result.AwayTeamId;
    }
}