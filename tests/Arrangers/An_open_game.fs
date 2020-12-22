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
