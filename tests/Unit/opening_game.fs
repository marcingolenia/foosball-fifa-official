module opening_game

open System
open Arrangers
open FsUnit.Xunit
open Xunit
open Foosball.Game
open A_team
open Foosball

[<Fact>]
let ``GIVEN rules, startDate and gameId WHEN game is opened THEN the scores list has empty set. The date, rules and id are set - the game is opened.`` () =
  // Arrange
  let startDate = DateTime.Now
  let rules = { MaxSetPoints = 10uy; MaxSets = 2uy }
  let gameId = 100 |> GameId
  let teams = (``A team`` Yellow |> fst, ``A team`` Black |> fst)
  // Act
  let game = openGame rules teams startDate gameId |> function | Ok game -> game | Error error -> failwith $"$A{error}"
  // Assert
  game.Score |> should haveLength 1
  game.Id |> should equal gameId
  game.Rules |> should equal game.Rules
  game.StartedAt |> should equal startDate
  game.Teams |> should equal teams
