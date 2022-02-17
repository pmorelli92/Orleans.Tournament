namespace Tournament.Domain.Teams;

public partial class TeamState
{
    public Guid Id { get; set; }
    public bool Created { get; set; }
    public string Name { get; set; }
    public List<string> Players { get; set; }
    public List<Guid> Tournaments { get; set; }

    public TeamState()
    {
        Name = string.Empty;
        Players = Enumerable.Empty<string>().ToList();
        Tournaments = Enumerable.Empty<Guid>().ToList();
    }

    public TeamState(
        Guid id,
        bool created,
        string name,
        List<string> players,
        List<Guid> tournaments)
    {
        Id = id;
        Created = created;
        Name = name;
        Players = players;
        Tournaments = tournaments;
    }

    public void Apply(TeamCreated evt)
    {
        Id = evt.TeamId;
        Name = evt.Name;
        Created = true;
    }

    public void Apply(PlayerAdded evt)
        => Players.Add(evt.Name);

    public void Apply(TournamentJoined evt)
        => Tournaments.Add(evt.TournamentId);
}