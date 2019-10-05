namespace Orleans.Tournament.Domain.Tournaments
open System

type MatchSummary =
    { LocalGoals: int
      AwayGoals: int }

type MatchInfo =
    { LocalTeamId:Guid
      AwayTeamId:Guid
      MatchSummary: MatchSummary option }
    member x.SetResult (matchSummary : MatchSummary) =
        { LocalTeamId = x.LocalTeamId;
          AwayTeamId = x.AwayTeamId;
          MatchSummary = Some matchSummary }
    member x.IsMatchInfo matchInfo =
        x.LocalTeamId = matchInfo.LocalTeamId && x.AwayTeamId = matchInfo.AwayTeamId

type Phase =
    { Matches : list<MatchInfo>
      Played: bool }
    member x.SetMatchResult matchInfo =
        match x.Played with
        | true -> x
        | false ->
            let newMatchList = matchInfo :: (x.Matches |> List.skipWhile (fun e -> e.IsMatchInfo matchInfo))
            { Matches = newMatchList;
              Played = newMatchList |> List.forall (fun e ->
                  match e.MatchSummary with
                  | Some _ -> true
                  | _ -> false ) }

type Fixture =
    { QuarterFinals : Phase
      SemiFinals : Phase Option
      Finals : Phase Option }
    static member private GeneratePhase (teams : list<Guid>) =
        let matches = seq {
                for i in 0 .. 2 .. teams.Length do
                yield { LocalTeamId = teams.[i] ; AwayTeamId = teams.[i+1]; MatchSummary = None } }
        { Matches = matches |> Seq.toList; Played = false }

    static member Create teams =
        let phase = Fixture.GeneratePhase(teams)
        { QuarterFinals = phase; SemiFinals = None; Finals = None }

    static member private MaybeGeneratePhase currentPhase =
        match currentPhase.Played with
        | false -> None
        | true ->
            let winningTeams =
                currentPhase.Matches
                |> List.map (fun e ->
                    let summary = e.MatchSummary.Value // Always exists since phase is played
                    match summary.LocalGoals > summary.AwayGoals with
                    | true -> e.LocalTeamId
                    | false -> e.AwayTeamId)
            Some (Fixture.GeneratePhase(winningTeams))

    member x.SetMatchResult matchInfo =
        match x.Finals with
        | Some f -> { QuarterFinals = x.QuarterFinals;
                      SemiFinals = x.SemiFinals;
                      Finals = Some (f.SetMatchResult matchInfo) }
        | None -> match x.SemiFinals with
                    | Some s ->
                          let semiFinals = s.SetMatchResult matchInfo;
                          { QuarterFinals = x.QuarterFinals;
                            SemiFinals = Some semiFinals;
                            Finals = Fixture.MaybeGeneratePhase semiFinals }
                    | None ->
                            let quarterFinals = x.QuarterFinals.SetMatchResult matchInfo;
                            { QuarterFinals = quarterFinals;
                              SemiFinals = Fixture.MaybeGeneratePhase quarterFinals;
                              Finals = None }


