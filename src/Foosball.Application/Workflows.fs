namespace Foosball.Application

open System
open Foosball
open Foosball.Game
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module OpenGameFlow =
  let toErrorMessage msg = Result.mapError(fun _ -> msg)
  let toErrorMessages msg = Result.mapError(fun _ -> [msg])

  let create (save: Game -> Async<Unit>)
             maxSets
             maxSetPoints
             (id: int64)
             team1
             team2
             =
    let teams = validation {
      let! team1Id = NotEmptyString.create team1 |> toErrorMessage $"{nameof team1} id cannot be empty."
      and! team2Id = NotEmptyString.create team2 |> toErrorMessage $"{nameof team2} id cannot be empty."
      return {| Team1 = team1Id; Team2 = team2Id |}
    }
    asyncResult {
      let! teams = teams
      let! openedGame = openGame
                         { MaxSets = maxSets; MaxSetPoints = maxSetPoints }
                         (teams.Team1, teams.Team2)
                         DateTime.UtcNow
                         (id |> GameId)
                        |> toErrorMessages $"Names must be unique, but {nameof team1}: {team1} and {nameof team2}: {team2} were given."
      return! save (openedGame |> OpenGame)
    }

module ScoreFlow =
  let score (readBy: GameId -> Async<Result<Game, string>>)
            (save: Game -> Async<Unit>)
            (id: int64)
            (footballersColor: string)
            (teamId: string)
            =
    let id = id |> GameId
    let footballersColor = (if footballersColor = null then "" else footballersColor).ToLowerInvariant()
    asyncResult {
      let! scoringTeam = teamId |> NotEmptyString.create
      let! teamColor = TeamColor.create footballersColor
      let! game = readBy id
      let! scoreResult =
        match game with
        | OpenGame game -> Ok (recordScore game (scoringTeam, teamColor) DateTime.UtcNow)
        | FinishedGame _ -> Error "Cannot make a score in finished game."
      do! save scoreResult
    }
