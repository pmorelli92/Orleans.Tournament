﻿namespace Tournament.Domain.Tournaments;

public record MatchResult(int LocalGoals, int AwayGoals, bool Played)
{
    public static MatchResult Empty
        => new MatchResult(0, 0, false);
}

public record Match(Guid LocalTeamId, Guid AwayTeamId, MatchResult MatchResult)
{
    public Match SetResult(MatchResult result) =>
        this with { MatchResult = result };
}

public record Phase(List<Match> Matches, bool Played)
{
    public bool IsEmpty => !Matches.Any();

    public Phase SetMatchResult(Match match)
    {
        if (Played)
            return this;

        // Find match index
        var index = Matches.FindIndex(e =>
            e.LocalTeamId == match.LocalTeamId &&
            e.AwayTeamId == match.AwayTeamId);


        Matches[index] = match;

        return this with
        {
            Played = Matches.All(e => e.MatchResult.Played)
        };
    }

    private static Phase GeneratePhase(List<Guid> teams)
    {
        var matches = new List<Match>();

        for (var i = 0; i < teams.Count; i++)
        {
            var local = teams[i];
            i++;
            var away = teams[i];

            matches.Add(new Match(local, away, MatchResult.Empty));
        }

        return new Phase(matches, false);
    }

    public static Phase Empty
        => new Phase(Enumerable.Empty<Match>().ToList(), false);

    public static Phase GenerateRandomPhase(List<Guid> teams, int seed)
    {
        var rnd = new Random(seed);
        return GeneratePhase(teams.OrderBy(e => rnd.Next()).ToList());
    }

    public static Phase MaybeGenerateNewPhaseFromCurrent(Phase current)
    {
        if (!current.Played)
            return Phase.Empty;

        var winners = current.Matches.Select(e =>
            e.MatchResult.LocalGoals > e.MatchResult.AwayGoals
                ? e.LocalTeamId
                : e.AwayTeamId
        ).ToList();

        return GeneratePhase(winners);
    }
}

public record Fixture(Phase Quarter, Phase Semi, Phase Final)
{
    public static Fixture Empty
        => new(Phase.Empty, Phase.Empty, Phase.Empty);
    public static Fixture Create(List<Guid> teams, int seed)
        => new(Phase.GenerateRandomPhase(teams, seed), Phase.Empty, Phase.Empty);

    public Fixture SetMatchResult(Match match)
    {
        if (!Final.IsEmpty)
        {
            return this with { Final = Final.SetMatchResult(match) };
        }
        else if (!Semi.IsEmpty)
        {
            var phase = Semi.SetMatchResult(match);
            return this with
            {
                Final = Phase.MaybeGenerateNewPhaseFromCurrent(phase),
                Semi = phase
            };
        }
        else
        {
            var phase = Quarter.SetMatchResult(match);
            return this with
            {
                Semi = Phase.MaybeGenerateNewPhaseFromCurrent(phase),
                Quarter = phase
            };
        }
    }
}
