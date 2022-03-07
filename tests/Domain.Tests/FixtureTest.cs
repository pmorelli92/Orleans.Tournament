using Tournament.Domain.Tournaments;
using Xunit;

namespace Tournament.Domain.Tests;

public class FixtureTests
{
    [Fact]
    public void SetMatchResult_Should_Preserve_Index_Position()
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

        var expected = sut.Quarter.Matches[0];
        var expectedLocalGoals = 2;
        var expectedAwayGoals = 0;

        var setMatch = new Match(
            expected.LocalTeamId,
            expected.AwayTeamId,
            new MatchResult(expectedLocalGoals, expectedAwayGoals));

        sut = sut.SetMatchResult(setMatch);

        // The index should be the same
        var actual = sut.Quarter.Matches[0];

        Assert.Equal(actual.LocalTeamId, expected.LocalTeamId);
        Assert.Equal(actual.AwayTeamId, expected.AwayTeamId);
        Assert.NotNull(actual.MatchResult);
        Assert.Equal(actual.MatchResult!.LocalGoals, expectedLocalGoals);
        Assert.Equal(actual.MatchResult.AwayGoals, expectedAwayGoals);
    }
}
