namespace Orleans.Tournament.Domain.Teams
open Orleans.Tournament.Domain.Teams.Events
open System

type TeamState =
    val mutable Id:Guid
    val mutable Created:bool
    val mutable Name:string
    val mutable Players:List<string>
    val mutable Tournaments:List<Guid>
    new() = {
        Id = Guid.Empty;
        Created = false;
        Name = String.Empty;
        Players = []
        Tournaments = []
    }
    member x.Apply (teamCreated : TeamCreated) =
        x.Created <- true;
        x.Id <- teamCreated.TeamId;
        x.Name <- teamCreated.Name;
    member x.Apply (playerAdded : PlayerAdded) =
        x.Players = playerAdded.Name :: x.Players
    member x.Apply (tournamentJoined : TournamentJoined) =
        x.Tournaments = tournamentJoined.TournamentId :: x.Tournaments;
