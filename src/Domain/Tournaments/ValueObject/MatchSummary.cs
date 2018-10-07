using System;
using Newtonsoft.Json;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class MatchInfo
    {
        public Guid LocalTeamId { get; }

        public Guid AwayTeamId { get; }

        public MatchSummary MatchSummary { get; }

        [JsonConstructor]
        public MatchInfo(Guid localTeamId, Guid awayTeamId, MatchSummary matchSummary)
        {
            LocalTeamId = localTeamId;
            AwayTeamId = awayTeamId;
            MatchSummary = matchSummary;
        }

        public MatchInfo(Guid localTeamId, Guid awayTeamId)
            : this(localTeamId, awayTeamId, null)
        {
        }

        public MatchInfo SetResult(int localGoals, int awayGoals)
            => new MatchInfo(LocalTeamId, AwayTeamId, new MatchSummary(localGoals, awayGoals));

        public bool ResultRelatesToMatch(MatchResult result)
            => LocalTeamId == result.LocalTeamId && AwayTeamId == result.AwayTeamId;
    }

    public class MatchSummary
    {
        public int LocalGoals { get; }

        public int AwayGoals { get; }

        public MatchSummary(int localGoals, int awayGoals)
        {
            LocalGoals = localGoals;
            AwayGoals = awayGoals;
        }
    }
}