namespace Foosball.Application

open System
open Foosball
open FsToolkit.ErrorHandling

module OpenGameFlow =
    let create (id: int64)
               rules
               (save: Game -> Async<Unit>) =
        let openedGame = openGame rules
                                  DateTime.UtcNow
                                  (id |> GameId) |> Game.OpenGame
        async { do! save openedGame }

module ScoreFlow =
  let score (id: int64)
            (footballersColor: string)
            (teamId: string)
            (readBy: GameId -> Async<Game>)
            (save: Game -> Async<Unit>) =
    let id = id |> GameId
    asyncResult {
      let! footballersColor =
        match footballersColor with
        | "yellow" -> Ok Yellow
        | "black" -> Ok Black
        | _ -> Error "Invalid footbalers color; acceptable values are: yellow, black"
      let! game = readBy id
      let! scoreResult =
        match game with
        | OpenGame game -> Ok (recordScore game (TeamId teamId, footballersColor) DateTime.UtcNow)
        | FinishedGame _ -> Error "Cannot make a score in finished game."
      do! save scoreResult
    }
