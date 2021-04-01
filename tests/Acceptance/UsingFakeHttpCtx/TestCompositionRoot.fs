namespace Acceptance

open System
open Settings
open Foosball.Api.CompositionRoot

module TestCompositionRoot =
  let testSettings: Settings =
      { SqlConnectionString = "Host=localhost;User Id=postgres;Password=Secret!Passw0rd;Database=foosball;Port=5432"
        FoosballRules = {MaxSets = 2uy; MaxSetPoints = 10uy}
        IdGeneratorSettings =
            { GeneratorId = 555
              Epoch = DateTimeOffset.Parse "2020-10-01 12:30:00"
              TimestampBits = byte 41
              GeneratorIdBits = byte 10
              SequenceBits = byte 12 } }

  let composeRoot tree = Root.compose tree
  let testTrunk = Trunk.compose testSettings

  let ``replace ReadGameBy`` substitute (trunk: Trunk.Trunk) =
    { trunk with ReadGameBy = substitute }

  let ``replace GamesQuery`` substitute (trunk: Trunk.Trunk) =
    { trunk with GamesQuery = substitute }

  let ``replace GameQuery`` substitute (trunk: Trunk.Trunk) =
    { trunk with GameQuery = substitute }

  let ``replace UpdateGame`` substitute (trunk: Trunk.Trunk) =
    { trunk with UpdateGame = substitute }

  let ``replace InsertGame`` substitute (trunk: Trunk.Trunk) =
    { trunk with InsertGame = substitute }
