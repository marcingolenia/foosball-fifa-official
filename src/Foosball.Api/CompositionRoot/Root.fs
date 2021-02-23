namespace Foosball.Api.CompositionRoot

open Foosball.Application
open Settings
open Trunk

module Root =
  type Root =
    { CreateGame: int64 -> string -> string -> Async<Result<Unit, string list>>
      ListGames: Async<seq<Queries.GameOverview>>
      ReadGameBy: int64 -> Async<Queries.GameDetails>
      Score: int64 -> string -> string -> Async<Result<unit, string list>>
      GenerateId: unit -> int64 }

  let compose (trunk: Trunk) =
    { ListGames = trunk.GamesQuery
      ReadGameBy = trunk.GameQuery
      CreateGame =
        OpenGameFlow.create
          trunk.InsertGame
          trunk.Settings.FoosballRules.MaxSets
          trunk.Settings.FoosballRules.MaxSetPoints
      Score = ScoreFlow.score trunk.ReadGameBy trunk.UpdateGame
      GenerateId = trunk.GenerateId }
