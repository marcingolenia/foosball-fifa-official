module making_points_within_set

open System
open Arrangers.A_team
open Arrangers.An_open_game
open FsUnit.Xunit
open Foosball.Game
open Foosball
open Xunit


[<Fact>]
let ``GIVEN freshly opened game WHEN recordScore for yellow's THEN scores list contains set which contains 1 point scored by yellow team`` () =
  // Arrange
  let yellowTeam = ``A team`` Yellow
  let scoredAt = DateTime.UtcNow
  // Act
  let game = recordScore ``An open game`` yellowTeam scoredAt
  // Assert
  match game with
  | FinishedGame _ -> failwith "Game still should be open!"
  | OpenGame game ->
      game.Score |> should haveLength 1
      game.Score.Head |> should haveLength 1
      game.Score.Head |> should contain { By = yellowTeam; At = scoredAt }

[<Fact>]
let ``GIVEN freshly opened game WHEN recordScore for black's THEN scores list contains set which contains 1 point scored by black team`` () =
  // Arrange
  let blackTeam = ``A team`` Black
  let scoredAt = DateTime.UtcNow
  // Act
  let game = recordScore ``An open game`` blackTeam scoredAt
  // Assert
  match game with
  | FinishedGame _ -> failwith "Game still should be open!"
  | OpenGame game ->
      game.Score |> should haveLength 1
      game.Score.Head |> should haveLength 1
      game.Score.Head |> should contain { By = blackTeam; At = scoredAt }

[<Fact>]
let ``GIVEN open game with 2:2 score WHEN recordScore for black's team THEN the resulting game score is 3:2  - black's team leads`` () =
  // Arrange
  let blackTeam = ``A team`` Black
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game`` |> ``add points in current set`` 2 blackTeam
                     |> ``add points in current set`` 2 (``A team`` Yellow)
  // Act
  let gameAfterScore = recordScore gameBeforeScore blackTeam scoredAt
  // Assert
  let fstSet =
    match gameAfterScore with
    | FinishedGame _ -> failwith "Game still should be open!"
    | OpenGame game -> game.Score.Head
  let points = fstSet |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Black].Length |> should equal 3
  points.[Yellow].Length |> should equal 2

[<Fact>]
let ``GIVEN open game with 2:2 score WHEN recordScore for yellow's team THEN the resulting game score is 2:3  - yellow's team leads`` () =
  // Arrange
  let yellowTeam = ``A team`` Yellow
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game`` |> ``add points in current set`` 2 yellowTeam
                     |> ``add points in current set`` 2 (``A team`` Black)
  // Act
  let gameAfterScore = recordScore gameBeforeScore yellowTeam scoredAt
  // Assert
  let fstSet =
    match gameAfterScore with
    | FinishedGame _ -> failwith "Game still should be open!"
    | OpenGame game -> game.Score.Head
  let points = fstSet |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Yellow].Length |> should equal 3
  points.[Black].Length |> should equal 2
