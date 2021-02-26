namespace Foosball

open System

module Game =
  type GameId = bigint
  type Errors =
    | TeamsMustBeUnique
  type Rules = { MaxSetPoints: byte; MaxSets: byte }
  type TeamId = NotEmptyString
  type Score = { By: TeamId * TeamColor; At: DateTime }
  type SetScores = { Number: byte; Scores: Score list }
  type OpenGame =
    { Id: GameId
      Teams: TeamId * TeamId
      StartedAt: DateTime
      Rules: Rules
      Score: Score list list }
  type FinishedGame =
    { Id: GameId
      Teams: TeamId * TeamId
      StartedAt: DateTime
      FinishedAt: DateTime
      Rules: Rules
      Score: Score list list }

  type Game =
      | OpenGame of OpenGame
      | FinishedGame of FinishedGame

  let private isGameWon (game: OpenGame) scoringTeamId =
      let maxPoints = int game.Rules.MaxSets * int game.Rules.MaxSetPoints
      game.Score |> List.concat |> List.groupBy(fun score -> score.By |> fst)
                                |> List.exists(fun(teamId, score) -> teamId = scoringTeamId
                                                                     && score.Length = maxPoints - 1)
  let (|SetWon|GameWon|SetInPlay|) (game: OpenGame, scoringTeam) =
      let isSetWon =  game.Score |> List.last
                                 |> List.groupBy(fun score -> score.By)
                                 |> List.exists(fun (team, score) -> team = scoringTeam
                                                                      && score.Length = int game.Rules.MaxSetPoints - 1)
      let isGameWon = isSetWon && isGameWon game (scoringTeam |> fst)
      match (isGameWon, isSetWon) with
      | (true, _) -> GameWon
      | (false, true) -> SetWon
      | _ -> SetInPlay

  let recordScore (game: OpenGame) (scoringTeam: TeamId * TeamColor) scoredAt: Game =
      let finishedSets = game.Score.[..^1]
      let currentSetWithNewPoint = [(game.Score |> List.last) @ [{ By = scoringTeam; At = scoredAt }]]
      match (game, scoringTeam) with
      | SetInPlay -> { game with Score = finishedSets @ currentSetWithNewPoint } |> Game.OpenGame
      | SetWon -> { game with Score = finishedSets @ currentSetWithNewPoint @ [[]] } |> Game.OpenGame
      | GameWon -> {
                       Id = game.Id
                       StartedAt = game.StartedAt
                       Rules = game.Rules
                       Teams = game.Teams
                       FinishedAt = scoredAt
                       Score = finishedSets @ currentSetWithNewPoint
                   } |> Game.FinishedGame

  let openGame rules teams startedAt gameId =
    match teams with
    | (t1, t2) when t1 = t2 -> Errors.TeamsMustBeUnique |> Error
    | _ -> { Id = gameId
             StartedAt = startedAt
             Teams = teams
             Rules = rules
             Score = [ [] ] } |> Ok
