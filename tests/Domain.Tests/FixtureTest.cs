using Tournament.Domain.Tournaments;
using Xunit;

namespace Tournament.Domain.Tests;

public class FixtureTests 
{
    [Fact]
    public void PlayMatch() 
    {
        var teams = new List<Guid>() {
            new Guid("10000000-0000-0000-0000-000000000000"),
            new Guid("20000000-0000-0000-0000-000000000000"),
            new Guid("30000000-0000-0000-0000-000000000000"),
            new Guid("40000000-0000-0000-0000-000000000000"),
            new Guid("50000000-0000-0000-0000-000000000000"),
            new Guid("60000000-0000-0000-0000-000000000000"),
            new Guid("70000000-0000-0000-0000-000000000000"),
            new Guid("80000000-0000-0000-0000-000000000000"),
        };

        var sut = Fixture.Create(teams, 1);


        var firstBracket = sut.Quarter.Matches[0];

        var setMatch = new Match(
            firstBracket.LocalTeamId,
            firstBracket.AwayTeamId,
            new MatchResult(2, 0));

        Console.WriteLine(sut.Quarter.Matches);

        sut = sut.SetMatchResult(setMatch);

        Console.WriteLine(sut.Quarter.Matches);
    }
}