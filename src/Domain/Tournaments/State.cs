namespace Tournament.Domain.Tournaments;

public partial class TournamentState
{
    public Guid Id { get; set; }
    public bool Created { get; set; }
    public string Name { get; set; }
    public List<Guid> Teams { get; set; }
    public Fixture? Fixture { get; set; }

    public TournamentState()
    {
        Name = string.Empty;
        Teams = Enumerable.Empty<Guid>().ToList();
    }

    public void Apply(TournamentCreated evt)
    {
        Id = evt.TournamentId;
        Created = true;
    }

    public void Apply(TeamAdded evt)
        => Teams.Add(evt.TeamId);

    public void Apply(TournamentStarted evt)
        => Fixture = Fixture.Create(Teams, evt.Seed);

    public void Apply(MatchResultSet evt)
        => Fixture = Fixture!.SetMatchResult(evt.Match);
}