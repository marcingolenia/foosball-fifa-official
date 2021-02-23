namespace Arrangers

open System
open Foosball
open Foosball.Game
open A_team

module A_finished_game =

  let ``A finished game won by`` winningTeam =
    let wonSet = [ for _ in 1 .. 10  -> { By = winningTeam; At = DateTime.UtcNow } ]
    let loosingTeam = winningTeam |> function | (_, Yellow) -> ``A team`` Black | _ -> ``A team`` Yellow
    let lostSet = [ for _ in 1 .. 10  -> { By = loosingTeam; At = DateTime.UtcNow } ]
    { Id = 100 |> GameId
      Rules = { MaxSetPoints = 10uy; MaxSets = 2uy }
      Teams = (loosingTeam |> fst, winningTeam |> fst)
      StartedAt = DateTime.UtcNow.AddMinutes(-5.0)
      FinishedAt = DateTime.UtcNow
      Score = [ wonSet; lostSet; wonSet; ]
    }

  let ``with id set to`` gameId game : FinishedGame =
    { game with Id = gameId }

  let ``with StartedAt and FinishedAt`` startedAt finishedAt game : FinishedGame =
    { game with StartedAt = startedAt; FinishedAt = finishedAt }
