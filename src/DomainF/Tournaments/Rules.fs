module Orleans.Tournament.Domain.Tournaments.Rules
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Tournaments

let TournamentExists (state : TournamentState) =
    match state.Created with
    | true -> Ok ()
    | false -> Error BusinessErrors.TournamentDoesNotExist

let TournamentDoesNotExists (state : TournamentState) =
    match state.Created with
    | false -> Ok ()
    | true -> Error BusinessErrors.TournamentAlreadyExist

let TeamExistsForward exists =
    match exists with
    | true -> Ok ()
    | false -> Error BusinessErrors.TeamDoesNotExist

let TeamIsNotAdded (state : TournamentState) teamId =
    match state.Teams |> List.tryFind (fun e -> e = teamId) with
    | Some _ -> Error BusinessErrors.TeamIsAlreadyAdded
    | None -> Ok ()

let LessThanEightTeams (state : TournamentState) =
    match state.Teams.Length with
    | c when c < 8 -> Ok ()
    | _ -> Error BusinessErrors.TournamentHasMoreThanEightTeams

let EightTeamsToStartTournament (state : TournamentState) =
    match state.Teams.Length = 8 with
    | true -> Ok ()
    | false -> Error BusinessErrors.CantStartTournamentWithLessThanEightTeams

let TournamentIsStarted (state : TournamentState) =
    match state.Fixture with
    | Some _ -> Ok ()
    | None -> Error BusinessErrors.TournamentIsNotStarted

let TournamentIsNotStarted (state : TournamentState) =
    match state.Fixture with
    | None -> Ok ()
    | Some _ -> Error BusinessErrors.TournamentAlreadyStarted

let MatchExistsAndIsNotPlayed (state : TournamentState) matchInfo =
    let currentPhase fixture =
        match fixture.Finals with
        | Some f -> f
        | None ->
            match fixture.SemiFinals with
            | Some sf -> sf
            | None -> fixture.QuarterFinals

    match state.Fixture with
    | Some f ->
        let c = currentPhase f
        let m = c.Matches |> List.tryFind (fun e -> e.IsMatchInfo matchInfo)
        match m with
        | Some m ->
            match m.MatchSummary with
            | None -> Ok ()
            | Some _ -> Error BusinessErrors.MatchAlreadyPlayed
        | None -> Error BusinessErrors.MatchDoesNotExist
    | None -> Error BusinessErrors.TournamentIsNotStarted

let MatchIsNotDraw (matchSummary : MatchSummary) =
    if matchSummary.LocalGoals = matchSummary.AwayGoals then true
    else false




