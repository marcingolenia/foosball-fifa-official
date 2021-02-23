module api_successes_with_hateoas

open System
open Acceptance
open Foosball.Game
open Foosball
open Foosball.Api
open Xunit
open FSharp.Control.Tasks.V2
open TestCompositionRoot
open HttpContext
open Arrangers.An_open_game
open Arrangers.A_team
open FsUnit.Xunit
open Foosball.Application

[<Fact>]
let ``GIVEN two teams names WHEN createGameHandler THEN response contains location header with game location AND links to game details and score action.`` () =
  // Arrange
  let mutable persistedGame: Game option = None
  let httpRequest = buildMockHttpContext () |> writeToBody { Team1 = "Team A1"; Team2 = "Team B1" }
  let root = testTrunk |> ``replace InsertGame`` (fun game -> async { persistedGame <- Some game; return () })
                       |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.createGameHandler root.CreateGame root.GenerateId next httpRequest
      return httpResponse
    } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  let actualGame = persistedGame.Value |> function | OpenGame game -> game | _ -> failwith "Game should be open"
  let links = httpResponse.Response |> deserializeResponse<Link list>
  httpResponse.Response.Headers.["Location"] |> should haveSubstring $"games/{actualGame.Id}"
  links |> should contain { Rel = "self"; Href = $"/games/{actualGame.Id}" }
  links |> should contain { Rel = "all"; Href = $"/games" }
  links.Length |> should equal 2

[<Fact>]
let ``GIVEN open and finished games WHEN listGamesHandler THEN response contains score action links for open games only AND action link fo getting game details AND games results are presenting persisted games.`` () =
  // Arrange
  let games = [ 1 .. 5 ] |> Seq.map (fun index ->
    let finishedAt = index % 2 |> function | 0 -> Some DateTime.Today | _ -> None
    { Id = int64 index
      StartedAt = DateTime.Today
      FinishedAt = finishedAt
      Team1 = "Team1"
      Team2 = "Team2"
      Team1Score = 1
      Team2Score = 2 }: Queries.GameOverview)
  let httpRequest = buildMockHttpContext () |> writeToBody { Team1 = "Team A1"; Team2 = "Team B1" }
  let root = testTrunk |> ``replace GamesQuery`` (async { return games }) |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.listGamesHandler root.ListGames next httpRequest
      return httpResponse } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  let linkedGames = httpResponse.Response |> deserializeResponse<LinkedResult<Queries.GameOverview> seq>
  (linkedGames, games) ||> Seq.iter2(fun linkedGame game ->
    linkedGame.Result |> should equal game
    linkedGame.Links |> should contain { Rel = "self"; Href = $"/games/{game.Id}" }
    match linkedGame.Result.FinishedAt with
    | Some _ -> ()
    | None _ -> linkedGame.Links |> should contain { Rel = "score"; Href = $"/games/{game.Id}/score" }
    )

[<Fact>]
let ``GIVEN open game WHEN readGameByHandler THEN response contains score action link AND link to all games.`` () =
  // Arrange
  let game: Queries.GameDetails = {
    Id = int64 1
    MaxSets = 2
    MaxSetPoints = 10
    StartedAt = DateTime.Today
    FinishedAt = None
    Team1 = "Team 1"
    Team2 = "Team 2"
    Score = [({ SetNo = 1
                Team = "Team 1"
                ScoredWith = "Yellow"
                ScoredAt = DateTime.Today
                }: Queries.Score)]
  }
  let httpRequest = buildMockHttpContext () |> writeToBody { Team1 = "Team A1"; Team2 = "Team B1" }
  let root = testTrunk |> ``replace GameQuery`` (fun _ -> async { return game }) |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.readGameByHandler root.ReadGameBy game.Id next httpRequest
      return httpResponse
      } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  let linkedGame = httpResponse.Response |> deserializeResponse<LinkedResult<Queries.GameDetails>>
  linkedGame.Links |> should contain { Rel = "score"; Href = $"/games/{game.Id}/score" }
  linkedGame.Links |> should contain { Rel = "all"; Href = $"/games" }
  linkedGame.Links.Length |> should equal 2
  linkedGame.Result |> should equal game

[<Fact>]
let ``GIVEN open game WHEN scoreHandler THEN the game score is updated AND link to game is returned.`` () =
  // Arrange
  let gameId = int64 10
  let (team1, team2) = (``A team`` Yellow, ``A team`` Black)
  let mutable gameAfterUpdate: Game option = None
  let gameBeforeUpdate = ``An open game``
                         |> ``with teams`` (team1 |> fst, team2 |> fst)
                         |> ``with id set to`` (gameId |> GameId)
                         |> ``add points in current set`` 5 team1
  let httpRequest = buildMockHttpContext () |> writeToBody { Team = team1 |> fst |> NotEmptyString.value; Color = "Yellow" }
  let root = testTrunk |> ``replace ReadGameBy`` (fun _ -> async { return gameBeforeUpdate |> OpenGame })
                       |> ``replace UpdateGame`` (fun game -> async { gameAfterUpdate <- Some game; return () })
                       |> composeRoot
  let httpResponse =
    task {
      // Act
      let! httpResponse = HttpHandler.scoreHandler root.Score gameId next httpRequest
      return httpResponse } |> Async.AwaitTask |> Async.RunSynchronously |> Option.get
  // Assert
  let gameAfterUpdate = gameAfterUpdate.Value |> function | OpenGame game -> game | _ -> failwith "Game should be open"
  let links = httpResponse.Response |> deserializeResponse<Link list>
  links |> should contain { Rel = "up"; Href = $"/games/{gameId}" }
  links.Length |> should equal 1
  gameBeforeUpdate.Score.Head.Length |> should lessThan gameAfterUpdate.Score.Head.Length
