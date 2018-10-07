using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class Fixture
    {
        public Dictionary<BracketPhase, List<MatchSummary>> Brackets { get; }

        public bool FinalPhase { get; private set; }

        public Fixture(Dictionary<BracketPhase, List<MatchSummary>> brackets)
        {
            Brackets = brackets;
        }

        public void SetMatchResult(MatchResult matchResult)
        {
            var currentBracket = Brackets.Last().Value;
            var index = currentBracket.FindIndex(e => e.ResultRelatesToMatch(matchResult));
            currentBracket[index] = currentBracket[index].SetResult(matchResult.LocalGoals, matchResult.AwayGoals);
        }

        public void StartNextPhase()
        {
            if (Brackets.Last().Key == BracketPhase.Final)
                throw new NotSupportedException("The tournament is already on the finals");

            var currentBracket = Brackets.Last().Value;

            var teamList = currentBracket
                .Select(e => e.LocalGoals > e.AwayGoals ? e.LocalTeamId : e.AwayTeamId)
                .ToList();

            var bracket = GenerateBracket(teamList.ToImmutableList());
            Brackets.Add(bracket.phase, bracket.matches);

            if (bracket.phase == BracketPhase.Final) FinalPhase = true;
        }

        public static Fixture Create(IImmutableList<Guid> teams)
        {
            var bracket = GenerateBracket(teams);

            return new Fixture(new Dictionary<BracketPhase, List<MatchSummary>>
            {
                { bracket.phase, bracket.matches }
            });
        }

        public static (BracketPhase phase, List<MatchSummary> matches) GenerateBracket(IImmutableList<Guid> teamList)
        {
            var teamMatchList = new List<MatchSummary>();

            for (var i = 0; i < teamList.Count; i = i + 2)
            {
                teamMatchList.Add(new MatchSummary(
                    localTeamId: teamList.ElementAtOrDefault(i),
                    awayTeamId: teamList.ElementAtOrDefault(i + 1)));
            }

            var phase = GetPhase(teamMatchList.Count);
            return (phase, teamMatchList);
        }

        public static BracketPhase GetPhase(int numberOfMatches)
        {
            switch (numberOfMatches)
            {
                case 4: return BracketPhase.QuarterFinal;
                case 2: return BracketPhase.SemiFinal;
                case 1: return BracketPhase.Final;
                default: return default;
            }
        }
    }

    public enum BracketPhase
    {
        Final = 1,
        SemiFinal = 3,
        QuarterFinal = 4
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> @this)
            => @this.OrderBy(a => Guid.NewGuid());
    }
}