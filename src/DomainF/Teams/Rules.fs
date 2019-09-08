module Orleans.Tournament.Domain.Teams.Rules
open Constants

let TeamExists (teamState : TeamState) =
    match teamState.Created with
    | true -> Ok teamState
    | false -> Error BusinessErrors.TeamDoesNotExist

let TeamDoesNotExists (teamState : TeamState) =
    match teamState.Created with
    | false -> Ok teamState
    | true -> Error BusinessErrors.TeamAlreadyExist
