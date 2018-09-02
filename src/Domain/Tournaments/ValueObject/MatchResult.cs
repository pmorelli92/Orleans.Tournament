using System;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class MatchResult
    {
        public Guid LocalTeamId { get; }

        public int LocalGoals { get; }

        public Guid AwayTeamId { get; }

        public int AwayGoals { get; }

        public MatchResult(Guid localTeamId, int localGoals, Guid awayTeamId, int awayGoals)
        {
            LocalTeamId = localTeamId;
            LocalGoals = localGoals;
            AwayTeamId = awayTeamId;
            AwayGoals = awayGoals;
        }
    }
}