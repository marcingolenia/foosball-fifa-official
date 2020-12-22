module making_points_within_set

open System
open Arrangers.An_open_game
open FsUnit.Xunit
open Foosball
open Xunit

[<Fact>]
let ``GIVEN freshly opened game WHEN recordScore for yellow's THEN scores list contains set which contains 1 point scored by yellow team`` () =
  // Arrange
  let yellowTeam = (TeamId "Marcin & Piotr", Yellow)
  let scoredAt = DateTime.UtcNow
  // Act
  let game = recordScore ``An open game`` yellowTeam scoredAt
  // Assert
  match game with
  | Finished _ -> failwith "Game still should be open!"
  | Open game ->
      game.Score |> should haveLength 1
      game.Score.Head |> should haveLength 1
      game.Score.Head |> should contain { By = yellowTeam; At = scoredAt }

[<Fact>]
let ``GIVEN freshly opened game WHEN recordScore for black's THEN scores list contains set which contains 1 point scored by black team`` () =
  // Arrange
  let blackTeam = (TeamId "Destroyers", Black)
  let scoredAt = DateTime.UtcNow
  // Act
  let game = recordScore ``An open game`` blackTeam scoredAt
  // Assert
  match game with
  | Finished _ -> failwith "Game still should be open!"
  | Open game ->
      game.Score |> should haveLength 1
      game.Score.Head |> should haveLength 1
      game.Score.Head |> should contain { By = blackTeam; At = scoredAt }
