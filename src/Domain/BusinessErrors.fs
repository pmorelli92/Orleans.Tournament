module Orleans.Tournament.Domain.Helpers

[<Literal>]
let MemoryProvider = "MemoryProvider";
[<Literal>]
let WebSocketNamespace = "WebSocketNamespace";
[<Literal>]
let TeamNamespace = "TeamNamespace";
[<Literal>]
let TournamentNamespace = "TournamentNamespace";

type BusinessErrors =
      TeamDoesNotExist = 1
    | TeamAlreadyExist = 2
    | TournamentDoesNotExist = 3
    | TournamentAlreadyExist = 4
    | TeamIsAlreadyAdded = 5
    | TournamentHasMoreThanEightTeams = 6
    | CantStartTournamentWithLessThanEightTeams = 7
    | TournamentIsNotStarted = 8
    | MatchDoesNotExist = 9
    | MatchAlreadyPlayed = 10
    | DrawResultIsNotAllowed = 11
    | NotAllMatchesPlayed = 12
    | TournamentAlreadyOnFinals = 13
    | TournamentAlreadyStarted = 14

let (<&>) (f1 : Result<unit, BusinessErrors>) (f2 : Result<unit, BusinessErrors>) =
  match f1, f2 with
  | Error e1, _ -> Error e1
  | _, Error e2 -> Error e2
  | Ok _, Ok _ -> Ok ()