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

[<Fact>]
let ``GIVEN open game with 2:2 score WHEN recordScore for black's team THEN the resulting game score is 3:2  - black's team leads`` () =
  // Arrange
  let blackTeam = (TeamId "Destroyers", Black)
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game``
    |> ``add points for black's in 1st set`` 2
    |> ``add points for yellow's in 1st set`` 2
  // Act
  let gameAfterScore = recordScore gameBeforeScore blackTeam scoredAt
  // Assert
  let fstSet =
    match gameAfterScore with
    | Finished _ -> failwith "Game still should be open!"
    | Open game -> game.Score.Head
  let points = fstSet |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Black].Length |> should equal 3
  points.[Yellow].Length |> should equal 2

[<Fact>]
let ``GIVEN open game with 2:2 score WHEN recordScore for yellow's team THEN the resulting game score is 2:3  - yellow's team leads`` () =
  // Arrange
  let yellowTeam = (TeamId "Marcin & Piotr", Yellow)
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game``
    |> ``add points for black's in 1st set`` 2
    |> ``add points for yellow's in 1st set`` 2
  // Act
  let gameAfterScore = recordScore gameBeforeScore yellowTeam scoredAt
  // Assert
  let fstSet =
    match gameAfterScore with
    | Finished _ -> failwith "Game still should be open!"
    | Open game -> game.Score.Head
  let points = fstSet |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Yellow].Length |> should equal 3
  points.[Black].Length |> should equal 2
