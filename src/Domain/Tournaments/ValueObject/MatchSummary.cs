using System;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class MatchSummary
    {
        public Guid LocalTeamId { get; }

        public int LocalGoals { get; }

        public Guid AwayTeamId { get; }

        public int AwayGoals { get; }

        public MatchSummary(Guid localTeamId, int localGoals, Guid awayTeamId, int awayGoals)
        {
            LocalTeamId = localTeamId;
            LocalGoals = localGoals;
            AwayTeamId = awayTeamId;
            AwayGoals = awayGoals;
        }
    }
}