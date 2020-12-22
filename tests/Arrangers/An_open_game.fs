namespace Arrangers

open System
open Foosball

module An_open_game =
  let ``An open game`` =
    { Id = 100 |> GameId
      Rules = { MaxSetPoints = 10uy; MaxSets = 2uy }
      StartedAt = DateTime.Now
      Score = [ [] ] }

  let ``set rule saying that set points limit is`` maxSetPoints game: OpenGame =
    { game with Rules = { game.Rules with MaxSetPoints = maxSetPoints } }

  let ``set rule saying that won sets limit is`` maxWonSets game: OpenGame =
    { game with Rules = { game.Rules with MaxSets = maxWonSets } }

  let ``add points for yellow's in 1st set`` points game: OpenGame =
      { game with
            Score =
                [ game.Score.Head
                  @ ([ 1 .. points ]
                     |> List.map (fun _ ->
                         { By = ( TeamId "Yellow", Yellow)
                           At = DateTime.UtcNow })) ] }

  let ``add points for black's in 1st set`` points game: OpenGame =
      { game with
            Score =
                [ game.Score.Head
                  @ ([ 1 .. points ]
                     |> List.map (fun _ ->
                         { By = (TeamId "Black", Black)
                           At = DateTime.UtcNow })) ] }
