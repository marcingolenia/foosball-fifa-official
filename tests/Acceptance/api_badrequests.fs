module api_badrequests

open Acceptance
open Foosball
open Foosball.Api
open Microsoft.AspNetCore.Http
open Xunit
open FSharp.Control.Tasks.V2
open TestCompositionRoot
open HttpContext
open FsUnit.Xunit

[<Theory>]
[<InlineData("", "")>]
[<InlineData(null, null)>]
[<InlineData("Team", "Team")>]
let ``GIVEN two teams names that are the same WHEN createGameHandler THEN response is bad request.`` (team1, team2) =
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
  httpResponse.Response |> toString |> should haveSubstring "Team names must be unique."

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
