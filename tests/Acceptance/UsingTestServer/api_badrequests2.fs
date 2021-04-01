module api_badrequests2

open System.Net
open System.Net.Http
open Acceptance.UsingTestServer
open Foosball.Api
open Microsoft.AspNetCore.TestHost
open TestApi
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``GIVEN two teams names that are the same WHEN createGameHandler THEN response is bad request.`` () =
  // Arrange
  use api = new TestServer(selfHosted)
  let client = api.CreateClient()
  let newGameDto = { Team1 = "team1abc"; Team2 = "team1abc" }
  // Act
  let response = client.PostGame newGameDto
  // Assert
  response.StatusCode |> should equal HttpStatusCode.BadRequest
  response.Content
            |> HttpContent.ToString
            |> should haveSubstring $"Names must be unique, but team1: {newGameDto.Team1} and team2: {newGameDto.Team2} were given."
