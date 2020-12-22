namespace Arrangers

open System
open Foosball

module An_open_game =
  let ``An open game`` =
    { Id = 100 |> GameId
      Rules = { MaxSetPoints = 10uy; MaxSets = 2uy }
      StartedAt = DateTime.UtcNow
      Score = [ [] ] }

  let ``set rule saying that set points limit is`` maxSetPoints game: OpenGame =
    { game with Rules = { game.Rules with MaxSetPoints = maxSetPoints } }

  let ``with id set to`` gameId game : OpenGame =
    { game with Id = gameId }

  let ``set rule saying that won sets limit is`` maxWonSets game: OpenGame =
    { game with Rules = { game.Rules with MaxSets = maxWonSets } }

  let ``add points in current set`` points team (game: OpenGame): OpenGame =
      let currentSet = (game.Score |> List.last) @
                       [ for _ in 1 .. points -> { By = team; At = DateTime.UtcNow } ]
      match game.Score with
      | [] | [_] -> { game with Score = [currentSet] }
      | _ -> let previousSets = game.Score.[ 0 .. game.Score.Length - 2]
             { game with Score = previousSets @ [currentSet] }

  let ``prepend set won by`` team (game: OpenGame): OpenGame =
      let wonSet = [ for _ in 1 .. int game.Rules.MaxSetPoints -> { By = team; At = DateTime.UtcNow } ]
      { game with Score = [ wonSet ] @ game.Score }
