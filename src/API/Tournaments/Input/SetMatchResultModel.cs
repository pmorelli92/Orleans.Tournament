using System;

namespace Orleans.Tournament.API.Tournaments.Input
{
    public class SetMatchResultModel
    {
        public Guid LocalTeamId { get; set; }

        public int LocalGoals { get; set; }

        public Guid AwayTeamId { get; set; }

        public int AwayGoals { get; set; }
    }
}
