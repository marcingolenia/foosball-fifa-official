namespace Foosball.Application

open System
open Foosball
open FsToolkit.ErrorHandling

module OpenGameFlow =
    let create (save: Game -> Async<Unit>)
               maxSets
               maxSetPoints
               (id: int64)
               team1
               team2
               =
      let (team1, team2) =
        match (team1, team2) with
        | (null, null) -> ("", "")
        | (null, _) -> ("", team2)
        | (_, null) -> (team1, "")
        | _ -> (team1, team2)
      asyncResult {
          let! openedGame = openGame { MaxSets = maxSets; MaxSetPoints = maxSetPoints }
                                    (team1 |> TeamId, team2 |> TeamId)
                                    DateTime.UtcNow
                                    (id |> GameId)
          return! save (openedGame |> Game.OpenGame)
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
      let! footballersColor =
        match footballersColor with
        | "yellow" -> Ok Yellow
        | "black" -> Ok Black
        | _ -> Error "Invalid footballers color; acceptable values are: yellow, black"
      let! game = readBy id
      let! scoreResult =
        match game with
        | OpenGame game -> Ok (recordScore game (TeamId teamId, footballersColor) DateTime.UtcNow)
        | FinishedGame _ -> Error "Cannot make a score in finished game."
      do! save scoreResult
    }
