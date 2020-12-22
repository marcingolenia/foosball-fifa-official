module finishing_game

open System
open Foosball
open Xunit
open Arrangers.An_open_game
open FsUnit.Xunit

[<Fact>]
let ``GIVEN open game with 9:9 AND rule says that 10 scores is the limit AND the set is final WHEN recordScore for a team THEN the last set score is 10:9 and game is finished`` () =
  // Arrange
  let winningTeam = (TeamId "I kill", TeamColor.Black)
  let loosingTeam = (TeamId "Why we need to loose? :(", TeamColor.Yellow)
  let scoredAt = DateTime.UtcNow
  let gameBeforeScore =
    ``An open game``
    |> ``set rule saying that set points limit is`` 10uy
    |> ``set rule saying that won sets limit is`` 2uy
    |> ``prepend set won by`` winningTeam
    |> ``prepend set won by`` loosingTeam
    |> ``add points in current set`` 9 winningTeam
    |> ``add points in current set`` 9 loosingTeam
  // Act
  let gameAfterScore = recordScore gameBeforeScore winningTeam scoredAt
  // Assert
  let finalScore =
    match gameAfterScore with
    | FinishedGame game -> game.Score
    | OpenGame _ -> failwith "Game should be finished!"
  let finalSetPoints =
    finalScore
    |> List.last
    |> List.groupBy (fun score -> score.By |> snd)
    |> Map.ofList
  finalSetPoints.[Yellow].Length |> should equal 9
  finalSetPoints.[Black].Length |> should equal 10
  finalScore.Length |> should equal (gameBeforeScore.Score.Length)

[<Fact>]
let ``GIVEN open game with 9:9 in last set AND 2:2 in sets AND teams did change sides AND rule says that 10 scores is the limit AND the set is final WHEN recordScore for a team THEN the last set score is 10:9 and game is finished`` () =
  // Arrange
  let team1 = TeamId "I Love"
  let team2 = TeamId "Santa Claus"
  let scoredAt = DateTime.UtcNow

  let gameBeforeScore =
    ``An open game``
    |> ``set rule saying that set points limit is`` 10uy
    |> ``set rule saying that won sets limit is`` 3uy
    |> ``prepend set won by`` (team1, Yellow)
    |> ``prepend set won by`` (team2, Black)
    |> ``prepend set won by`` (team1, Black)// sides changed
    |> ``prepend set won by`` (team2, Yellow)
    |> ``add points in current set`` 9 (team1, Yellow)
    |> ``add points in current set`` 9 (team2, Black)
  // Act
  let gameAfterScore = recordScore gameBeforeScore (team1, Yellow) scoredAt
  // Assert
  let finishedGame =
    match gameAfterScore with
    | FinishedGame game -> game
    | OpenGame _ -> failwith "Game should be finished!"
  let totalScore =
    finishedGame.Score
    |> List.concat
    |> List.groupBy (fun score -> score.By |> snd)
    |> Map.ofList
  let finalSetPoints =
    finishedGame.Score
    |> List.last
    |> List.groupBy (fun score -> score.By |> snd)
    |> Map.ofList
  finalSetPoints.[Yellow].Length |> should equal 10
  finalSetPoints.[Black].Length |> should equal 9
  (totalScore.[Yellow].Length / int finishedGame.Rules.MaxSetPoints) |> should equal (int finishedGame.Rules.MaxSets)
  finishedGame.Score.Length |> should equal (gameBeforeScore.Score.Length)
