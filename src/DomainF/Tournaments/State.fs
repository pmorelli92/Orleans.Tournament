namespace Orleans.Tournament.Domain.Tournaments
open System

type TournamentState =
    val mutable Id:Guid
    val mutable Created:bool
    val mutable Name:string
    val mutable Teams:list<Guid>
    val mutable Fixture:Fixture option
    new() = {
        Id = Guid.Empty;
        Created = false;
        Name = String.Empty;
        Teams = []
        Fixture = None
    }
    member x.Apply (evt : TournamentCreated) =
        x.Created <- true;
        x.Id <- evt.TournamentId;
        x.Name <- evt.Name;
    member x.Apply (evt : TeamAdded) =
        x.Teams = evt.TeamId :: x.Teams
    member x.Apply (evt : TournamentStarted) =
        x.Fixture <- Some (Fixture.Create evt.Teams);
    member x.Apply (evt : MatchResultSet) =
        x.Fixture <- Some (x.Fixture.Value.SetMatchResult evt.MatchInfo);
