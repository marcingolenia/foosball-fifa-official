namespace Foosball.Api.CompositionRoot

open Foosball.Game
open Foosball.Application
open Foosball.PostgresPersistence
open Settings

module Trunk =
  type Trunk =
    { GamesQuery: Async<Queries.GameOverview seq>
      GameQuery: int64 -> Async<Queries.GameDetails>
      ReadGameBy: GameId -> Async<Game>
      UpdateGame: Game -> Async<unit>
      InsertGame: Game -> Async<unit>
      GenerateId: unit -> int64
      Settings: Settings }

  let compose (settings: Settings) =
    let createConnection = DapperFSharp.createSqlConnection settings.SqlConnectionString

    { GamesQuery = GameQueriesDao.list createConnection
      GameQuery = GameQueriesDao.readBy createConnection
      ReadGameBy = GameDao.readBy createConnection
      UpdateGame = GameDao.update createConnection
      InsertGame = GameDao.insert createConnection
      GenerateId = IdGenerator.create settings.IdGeneratorSettings
      Settings = settings }
