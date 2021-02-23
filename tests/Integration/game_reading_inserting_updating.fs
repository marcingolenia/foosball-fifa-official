module game_reading_inserting_updating

open Foosball.Game
open Foosball
open Foosball.PostgresPersistence.GameDao
open Xunit
open FsToolkit.ErrorHandling
open FsUnit.Xunit
open Arrangers.An_open_game
open Arrangers.A_team

[<Fact>]
let ``GIVEN open game WHEN inserted THEN after read it is fully restored`` () =
    // Arrange
    let id = (Toolbox.generateId() |> GameId)
    let expectedGame = ``An open game`` |> ``with id set to`` id  |> OpenGame
    asyncResult {
        // Act
        do! insert DbConnection.create expectedGame
        // Assert
        let! actualGame = readBy DbConnection.create id
        actualGame |> should equal expectedGame
    } |> Async.RunSynchronously

[<Fact>]
let ``GIVEN open game WHEN updated THEN after read it is has the updates`` () =
    // Arrange
    let id = (Toolbox.generateId() |> GameId)
    let gameToUpdate = ``An open game`` |> ``with id set to`` id  |> OpenGame
    asyncResult {
        // Act
        do! insert DbConnection.create gameToUpdate
        let expectedGame = ``An open game``
                           |> ``with id set to`` id
                           |> ``prepend set won by`` (``A team`` Yellow) |> OpenGame
        do! update DbConnection.create expectedGame
        // Assert
        let! actualGame = readBy DbConnection.create id
        actualGame |> should not' (be gameToUpdate)
        actualGame |> should equal expectedGame
    } |> Async.RunSynchronously
