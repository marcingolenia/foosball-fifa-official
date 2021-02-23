namespace Foosball.PostgresPersistence

open Foosball.Game
open DapperFSharp
open Thoth.Json.Net

module GameDao =
    let readBy createConnection (id: GameId) =
        async {
            use! connection = createConnection ()
            let id = id |> int64
            let! gameJson = connection |> sqlSingle<string> "SELECT data FROM games WHERE id = @id" {|id = id|}
            let game = Decode.Auto.fromString<Game>(gameJson, CaseStrategy.PascalCase, Extra.empty |> Extra.withBigInt)
            return game
        }

    let insert createConnection (game: Game) =
        async {
            use! connection = createConnection ()
            let id = int64 (game |> function | OpenGame game -> game.Id | FinishedGame game -> game.Id)
            let json = Encode.Auto.toString(4, game, CaseStrategy.PascalCase, Extra.empty |> Extra.withBigInt)
            do! connection |> sqlExecute "
            INSERT INTO games
            (id, data)
            VALUES(@Id, @Data::jsonb);" {| Id = id; Data = json |}
        }

    let update createConnection game =
        async {
            use! connection = createConnection ()
            let id = int64 (game |> function | OpenGame game -> game.Id | FinishedGame game -> game.Id)
            let json = Encode.Auto.toString(4, game, CaseStrategy.PascalCase, Extra.empty |> Extra.withBigInt)
            do! connection |> sqlExecute "
            UPDATE games
            SET data = @Data::jsonb
            WHERE id = @Id" {| Id = id; Data = json |}
        }
