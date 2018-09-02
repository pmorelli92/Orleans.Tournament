using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Snaelro.Domain.Tournaments.ValueObject
{
    public class Fixture
    {
        // Editing by ref need fields not property
        private Phase _finals;
        private Phase _semiFinals;
        private Phase _quarterFinals;

        public Phase Finals => _finals;
        public Phase SemiFinals => _semiFinals;
        public Phase QuarterFinals => _quarterFinals;

        public Fixture(Phase quarterFinals, Phase semiFinals, Phase finals)
        {
            _quarterFinals = quarterFinals;
            _semiFinals = semiFinals;
            _finals = finals;
        }

        public void SetMatchResult(MatchResult matchResult)
        {
            var currentPhase = GetCurrentPhase();
            var matches = currentPhase.Matches;
            var index = matches.FindIndex(e => e.ResultRelatesToMatch(matchResult));
            matches[index] = matches[index].SetResult(matchResult.LocalGoals, matchResult.AwayGoals);

            if (matches.All(e => e.MatchSummary != null))
                currentPhase.Played = true;

        }

        public void StartNextPhase()
        {
            var teamList = GetCurrentPhase().Matches
                .Select(e => e.MatchSummary.LocalGoals > e.MatchSummary.AwayGoals ? e.LocalTeamId : e.AwayTeamId)
                .ToList();

            GetNextPhase() = GeneratePhase(teamList);
        }

        public ref Phase GetCurrentPhase()
        {
            if (Finals.Matches.Any()) return ref _finals;
            if (SemiFinals.Matches.Any()) return ref _semiFinals;
            if (QuarterFinals.Matches.Any()) return ref _quarterFinals;

            throw new InvalidOperationException("This should not be happening");
        }

        public ref Phase GetNextPhase()
        {
            if (Finals.Matches.Any()) throw new NotSupportedException("The tournament is already on the finals");
            if (SemiFinals.Matches.Any()) return ref _finals;
            return ref _semiFinals;
        }

        public static Fixture Create(IImmutableList<Guid> teams)
        {
            var phase = GeneratePhase(teams);
            return new Fixture(phase, semiFinals: Phase.PlaceHolder, finals: Phase.PlaceHolder);
        }

        private static Phase GeneratePhase(IReadOnlyCollection<Guid> teamList)
        {
            var teamMatchList = new List<MatchInfo>();

            for (var i = 0; i < teamList.Count; i = i + 2)
            {
                teamMatchList.Add(new MatchInfo(
                    localTeamId: teamList.ElementAtOrDefault(i),
                    awayTeamId: teamList.ElementAtOrDefault(i + 1)));
            }

            return new Phase(teamMatchList);
        }
    }

    public class Phase
    {
        public List<MatchInfo> Matches { get; }

        public bool Played { get; internal set; }

        public Phase(List<MatchInfo> matches, bool played)
        {
            Matches = matches;
            Played = played;
        }

        internal Phase(List<MatchInfo> matches)
            : this(matches, played: false)
        {
        }

        internal static Phase PlaceHolder
            => new Phase(new List<MatchInfo>());
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> @this)
            => @this.OrderBy(a => Guid.NewGuid());
    }
}