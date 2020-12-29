module game_queries

open System
open Arrangers
open Foosball
open Xunit
open FsToolkit.ErrorHandling
open FsUnit.Xunit
open Arrangers.A_finished_game
open Arrangers.An_open_game
open Foosball.PostgresPersistence.GameDao
open Foosball.PostgresPersistence.GameQueriesDao

open type DateTime

[<Fact>]
let ``GIVEN a game WHEN readBy THEN game is returned with rules, scores`` () =
  // Arrange
  let teamA = ("TEAM A" |> TeamId, Yellow)
  let teamB = ("TEAM B" |> TeamId, Black)
  let gameId = Toolbox.generateId()
  let game = ``An open game``
              |> ``with id set to`` (gameId |> GameId)
              |> ``with teams`` (teamA |> fst, teamB |> fst)
              |> ``prepend set won by`` teamA
              |> ``prepend set won by`` teamB
              |> ``add points in current set`` 8 teamA
              |> ``add points in current set`` 2 teamB
  let gameDetails =
    async {
      do! insert DbConnection.create (game |> Game.OpenGame)
      // Act
      return! readBy DbConnection.create gameId
    } |> Async.RunSynchronously
  // Assert
  gameDetails.Id |> should equal gameId
  gameDetails.FinishedAt |> should equal None
  (TeamId gameDetails.Team1) |> should equal (teamA |> fst)
  (TeamId gameDetails.Team2) |> should equal (teamB |> fst)
  gameDetails.StartedAt.Date |> should equal game.StartedAt.Date
  gameDetails.MaxSets |> should equal (int game.Rules.MaxSets)
  gameDetails.MaxSetPoints |> should equal (int game.Rules.MaxSetPoints)
  gameDetails.Score |> Seq.length |> should equal (game.Score |> List.concat |> List.length)
  let teamAScore = gameDetails.Score |> Seq.where(fun score -> (TeamId score.Team) = (teamA |> fst)) |> Seq.length
  let teamBScore = gameDetails.Score |> Seq.where(fun score -> (TeamId score.Team) = (teamB |> fst)) |> Seq.length
  teamAScore |> should equal 18
  teamBScore |> should equal 12

[<Fact>]
let ``GIVEN open and finished games WHEN list THEN list of games is returned with scores, start dates, teams, finished games, sorted by start date descending`` () =
  // Arrange
  let winners = ("Winners" |> TeamId, Yellow)
  let gameIds = [for _ in 1 .. 4 -> Toolbox.generateId() |> GameId]
  let games = gameIds |> List.mapi(fun index id ->
    match index % 2 with
    | 0 -> ``An open game``
            |> ``prepend set won by`` winners
            |> ``with id set to`` id
            |> ``with StartedAt`` (UtcNow.AddMinutes(float -index))
            |> Game.OpenGame
    | _ -> ``A finished game won by`` winners
             |> A_finished_game.``with id set to`` id
             |> A_finished_game.``with StartedAt and FinishedAt`` (UtcNow.AddMinutes(float -index))
                                                                  (UtcNow.AddMinutes(float -index))
             |> Game.FinishedGame )
  let gamesList =
      async {
          let! _ = games |> List.map(insert DbConnection.create) |> Async.Parallel
          // Act
          return! list DbConnection.create
      } |> Async.RunSynchronously
  gamesList
    |> Seq.where(fun game -> gameIds |> List.contains (game.Id |> GameId) )
    |> Seq.pairwise
    |> Seq.forall(fun (a,b) -> a.StartedAt > b.StartedAt ) |> should equal true

