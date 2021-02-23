namespace Foosball.Application

open System
open Foosball
open Foosball.Game
open FsToolkit.ErrorHandling

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
  let score (readBy: GameId -> Async<Game>)
            (save: Game -> Async<Unit>)
            (id: int64)
            (footballersColor: string)
            (teamId: string)
            =
    let id = id |> GameId
    let footballersColor = (if footballersColor = null then "" else footballersColor).ToLowerInvariant()
    let scoringTeam = validation {
      let! scoringTeam = teamId |> NotEmptyString.create
      and! teamColor = TeamColor.create footballersColor
      return (scoringTeam, teamColor)
    }
    asyncResult {
      let! scoringTeam = scoringTeam
      let! game = readBy id
      let! newGame =
        match game with
        | OpenGame game -> recordScore game scoringTeam DateTime.UtcNow |> Ok
        | FinishedGame _ -> ["Cannot make a score in finished game."] |> Error
      do! save newGame
    }
