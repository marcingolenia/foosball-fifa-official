module api_badrequests

open Acceptance
open Foosball.Api
open Microsoft.AspNetCore.Http
open Xunit
open FSharp.Control.Tasks.V2
open TestCompositionRoot
open HttpContext
open System
open FsUnit.Xunit

[<Fact>]
let ``GIVEN two teams names that are the same WHEN createGameHandler THEN response is bad request.`` () =
  // Arrange
  let newGameDto = { Team1 = "team1abc"; Team2 = "team1abc" }
  let httpRequest = buildMockHttpContext () |> writeToBody newGameDto
  let root = testTrunk |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.createGameHandler root.CreateGame root.GenerateId next httpRequest
      return httpResponse
    } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  httpResponse.Response.StatusCode |> should equal StatusCodes.Status400BadRequest
  httpResponse.Response |> toString |> should haveSubstring $"Names must be unique, but team1: {newGameDto.Team1} and team2: {newGameDto.Team2} were given."

[<Theory>]
[<InlineData("", "")>]
[<InlineData(null, null)>]
[<InlineData("", "Team")>]
[<InlineData("Team", null)>]
[<InlineData("", null)>]
let ``GIVEN two teams names that are at least one is empty or null WHEN createGameHandler THEN response is bad request.`` (team1, team2) =
  // Arrange
  let httpRequest = buildMockHttpContext () |> writeToBody { Team1 = team1; Team2 = team2 }
  let root = testTrunk |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.createGameHandler root.CreateGame root.GenerateId next httpRequest
      return httpResponse
    } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  httpResponse.Response.StatusCode |> should equal StatusCodes.Status400BadRequest
  if not (String.IsNullOrWhiteSpace team1) then
    httpResponse.Response |> toString |> should haveSubstring "[\"team2 id cannot be empty.\"]"
  elif not (String.IsNullOrWhiteSpace team2) then
    httpResponse.Response |> toString |> should haveSubstring "[\"team1 id cannot be empty.\"]"
  else
    httpResponse.Response |> toString |> should haveSubstring "[\"team1 id cannot be empty.\",\"team2 id cannot be empty.\"]"

[<Theory>]
[<InlineData("")>]
[<InlineData(null)>]
[<InlineData("orange")>]
[<InlineData("-1")>]
let ``GIVEN team scores AND team color is not yellow or black WHEN scoreHandler THEN response is bad request.`` (color :string) =
  // Arrange
  let httpRequest = buildMockHttpContext () |> writeToBody { Team = "Team A1"; Color = color }
  let root = testTrunk |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.scoreHandler root.Score (int64 1) next httpRequest
      return httpResponse
    } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  httpResponse.Response.StatusCode |> should equal StatusCodes.Status400BadRequest
  httpResponse.Response |> toString |> should haveSubstring "Invalid footballers color; acceptable values are: yellow, black"
