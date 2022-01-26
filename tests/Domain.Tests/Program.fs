module SomeModule
open Orleans.Tournament.Domain.Helpers
open Orleans.Tournament.Domain.Teams
open Orleans.Tournament.Domain.Tournaments
open System
open System.Threading.Tasks

type DoucheGrain() =
    member x.TeamExistAsync() =
        Task.FromResult(true)
//        let wrap value = async { return value } |> Async.StartAsTask
//        wrap true

[<EntryPoint>]
let main argv =
    let d = new DoucheGrain()
    let strval2 =
        async {
            return! d.TeamExistAsync() |> Async.AwaitTask
        } |> Async.RunSynchronously
    Console.WriteLine(strval2)
    0

//    let trs = new TournamentState()
//    let someTeam = Guid.NewGuid()
//    trs.Created <- true
//
//    match
//        Rules.TournamentExists(trs) <&>
//        Rules.TeamExistsForward(true) <&>
//        Rules.TeamIsNotAdded trs someTeam <&>
//        Rules.LessThanEightTeams trs with
//    | Ok _ ->
//        Console.WriteLine("good")
//        1
//    | Error error ->
//        Console.WriteLine(error)
//        1

//        member x.AddTeamAsync cmd =
//            // There is no problem on validating on the controller using the
//            // read side tables, but this validation should be done on the domain
//            // as this is the entry point of truth
//            let teamGrain = x.GrainFactory.GetGrain<ITeamGrain>(cmd.TeamId);
//            let teamExist = teamGrain.TeamExistAsync() |> Async.AwaitTask |> Async.RunSynchronously
//
//            match
//                Rules.TournamentExists(x.State) <&>
//                Rules.TeamExistsForward(teamExist) <&>
//                Rules.TeamIsNotAdded x.State cmd.TeamId <&>
//                Rules.LessThanEightTeams x.State with
//              | Ok _ -> x.PersistPublishAsync (TeamAdded.From cmd)
//              | Error error -> x.EmitErrorAsync error cmd