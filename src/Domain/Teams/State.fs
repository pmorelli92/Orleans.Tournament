namespace Orleans.Tournament.Domain.Teams
open System

type TeamState =
    val mutable Id:Guid
    val mutable Created:bool
    val mutable Name:string
    val mutable Players:list<string>
    val mutable Tournaments:list<Guid>
    new() = {
        Id = Guid.Empty;
        Created = false;
        Name = String.Empty;
        Players = []
        Tournaments = []
    }
    new (id, created, name, players, tournaments) = {
        Id = id
        Created = created
        Name = name
        Players = players
        Tournaments = tournaments
    }
    member x.Apply (evt : TeamCreated) =
        x.Created <- true;
        x.Id <- evt.TeamId;
        x.Name <- evt.Name;
    member x.Apply (evt : PlayerAdded) =
        x.Players <- evt.Name :: x.Players
    member x.Apply (evt : TournamentJoined) =
        x.Tournaments <- evt.TournamentId :: x.Tournaments;
