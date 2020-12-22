module Foosball

open System

type GameId = bigint
type Rules = { MaxSetPoints: byte; MaxSets: byte }
type TeamColor = Yellow | Black // Official championship's colors
type TeamId = TeamId of string
type Score = { By: TeamId * TeamColor; At: DateTime }
type SetScores = { Number: byte; Scores: Score list }
type OpenGame = { Id: GameId; StartedAt: DateTime; Rules: Rules; Score: Score list list }

type FinishedGame =
  { Id: GameId
    StartedAt: DateTime
    FinishedAt: DateTime
    Rules: Rules
    Score: Score list list }

type Game =
  | Open of OpenGame
  | Finished of FinishedGame

let private findSetWinner scoringTeam maxSetPoints setScore =
    let isBlackToWin = setScore |> List.choose(fun score -> score.By |> snd |> function | Yellow _ -> Some 1 | _ -> None)
                                |> List.length = maxSetPoints - 1
    let isYellowToWin = setScore |> List.choose(fun score -> score.By |> snd |> function | Black _ -> Some 1 | _ -> None )
                                 |> List.length = maxSetPoints - 1
    match scoringTeam |> snd with
    | Yellow _ when isYellowToWin -> Some Yellow
    | Black _ when isBlackToWin -> Some Black
    | _ -> None

let (|SetWon|GameWon|SetInPlay|) (rules, score, scoringTeam) =
    let setWinningTeam = score |> List.last |> findSetWinner scoringTeam (int rules.MaxSetPoints)
    let isSetFinal = score.Length = int rules.MaxSets
    match (setWinningTeam, isSetFinal) with
    | (Some _, true) -> GameWon
    | (Some _, false) -> SetWon
    | _ -> SetInPlay

let recordScore (game: OpenGame) (scoringTeam: TeamId * TeamColor) scoredAt: Game =
    let finishedSets = game.Score.[.. game.Score.Length - 2]
    let currentSetWithNewPoint = [(game.Score |> List.last) @ [{ By = scoringTeam; At = scoredAt }]]
    match (game.Rules, game.Score, scoringTeam) with
    | SetInPlay -> { game with Score = finishedSets @ currentSetWithNewPoint } |> Game.Open
    | SetWon -> { game with Score = finishedSets @ currentSetWithNewPoint @ [[]] } |> Game.Open
    | GameWon -> {
                     Id = game.Id
                     StartedAt = game.StartedAt
                     Rules = game.Rules
                     FinishedAt = scoredAt
                     Score = [game.Score.Head @ [{ By = scoringTeam; At = scoredAt }]]
                 } |> Game.Finished

let openGame rules startedAt gameId = { Id = gameId; StartedAt = startedAt; Rules = rules; Score = [ [] ] }
