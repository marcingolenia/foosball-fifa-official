namespace Arrangers

open System
open Foosball

module A_finished_game =

  let ``A finished game won by`` winningTeam =
    let wonSet = [ for _ in 1 .. 10  -> { By = winningTeam; At = DateTime.UtcNow } ]
    let loosingTeam = ( TeamId "Other team that lost", winningTeam |> function | (_, Yellow) -> Black | _ -> Yellow)
    let lostSet = [ for _ in 1 .. 10  -> { By = loosingTeam; At = DateTime.UtcNow } ]
    { Id = 100 |> GameId
      Rules = { MaxSetPoints = 10uy; MaxSets = 2uy }
      Teams = (TeamId "Other team that lost", winningTeam |> fst)
      StartedAt = DateTime.UtcNow.AddMinutes(-5.0)
      FinishedAt = DateTime.UtcNow
      Score = [ wonSet; lostSet; wonSet; ]
    }

  let ``with id set to`` gameId game : FinishedGame =
    { game with Id = gameId }

  let ``with StartedAt and FinishedAt`` startedAt finishedAt game : FinishedGame =
    { game with StartedAt = startedAt; FinishedAt = finishedAt }
