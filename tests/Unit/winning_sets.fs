module winning_set

open System
open Arrangers.An_open_game
open FsUnit.Xunit
open Foosball
open Xunit

[<Fact>]
let ``GIVEN open game with 9:9 AND rule says that 10 scores is the limit WHEN recordScore for yellow's team THEN the first set score is 10:9 for yellow's and new set with empty score is a added to game`` () =
  // Arrange
  let yellowTeam = (TeamId "Destroyers", TeamColor.Yellow)
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game``
    |> ``set rule saying that set points limit is`` 10uy
    |> ``add points in current set`` 9 yellowTeam
    |> ``add points in current set`` 9 (TeamId "Whatever", TeamColor.Black)
  // Act
  let gameAfterScore = recordScore gameBeforeScore yellowTeam scoredAt
  // Assert
  let newScore =
    match gameAfterScore with
    | FinishedGame _ -> failwith "Game still should be open!"
    | OpenGame game -> game.Score
  let points = newScore.Head |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Yellow].Length |> should equal 10
  points.[Black].Length |> should equal 9
  newScore.Length |> should equal (gameBeforeScore.Score.Length + 1)

[<Fact>]
let ``GIVEN open game with 9:9 AND rule says that 10 scores is the limit WHEN recordScore for black's team THEN the first set score is 10:9 for black's and new set with empty scores is a added to game`` () =
  // Arrange
  let blackTeam = (TeamId "I kill", TeamColor.Black)
  let scoredAt = DateTime.UtcNow

  let gameBeforeScore =
    ``An open game``
    |> ``set rule saying that set points limit is`` 10uy
    |> ``add points in current set`` 9 blackTeam
    |> ``add points in current set`` 9 (TeamId "Destroyers", TeamColor.Yellow)
  // Act
  let gameAfterScore = recordScore gameBeforeScore blackTeam scoredAt
  // Assert
  let newScore =
    match gameAfterScore with
    | FinishedGame _ -> failwith "Game still should be open!"
    | OpenGame game -> game.Score
  let points = newScore.Head |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  points.[Yellow].Length |> should equal 9
  points.[Black].Length |> should equal 10
  newScore.Length |> should equal (gameBeforeScore.Score.Length + 1)

[<Fact>]
let ``GIVEN open game with 1:0 in sets for yellows AND current set points are 9:9 AND rule says that 10 scores is the limit AND rule says that 3 sets has to be won to win the game WHEN recordScore for yellow's team THEN the second set score is 10:9 for yellow's and new set with empty scores is a added to game`` () =
  // Arrange
  let yellowTeam = (TeamId "Destroyers", TeamColor.Yellow)
  let scoredAt = DateTime.UtcNow

  let gameBeforeScore =
    ``An open game``
    |> ``set rule saying that set points limit is`` 10uy
    |> ``set rule saying that won sets limit is`` 3uy
    |> ``prepend set won by`` yellowTeam
    |> ``add points in current set`` 9 (TeamId "Destroyers", TeamColor.Black)
    |> ``add points in current set`` 9 yellowTeam
  // Act
  let gameAfterScore = recordScore gameBeforeScore yellowTeam scoredAt
  // Assert
  let newScore =
    match gameAfterScore with
    | FinishedGame _ -> failwith "Game still should be open!"
    | OpenGame game -> game.Score
  let sndSetScore = newScore.[1] |> List.groupBy (fun score -> score.By |> snd) |> Map.ofList
  sndSetScore.[Yellow].Length |> should equal 10
  sndSetScore.[Black].Length |> should equal 9
  newScore.Length |> should equal 3
