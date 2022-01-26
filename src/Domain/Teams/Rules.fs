module Orleans.Tournament.Domain.Teams.Rules
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Teams

let TeamExists (state : TeamState) =
    match state.Created with
    | true -> Ok ()
    | false -> Error BusinessErrors.TeamDoesNotExist

let TeamDoesNotExists (state : TeamState) =
    match state.Created with
    | false -> Ok ()
    | true -> Error BusinessErrors.TeamAlreadyExist