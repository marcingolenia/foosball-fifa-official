module Foosball

open System

type GameId = bigint

type Footballers = | Yellow | Black // Official championship's colors
type Rules = { PointsInSet: byte; SetsToWinToBeWinner: byte }
type TeamName = TeamName of string
type Team = TeamName * Footballers
type Score = { By: Team; At: DateTime }
type SetScores = { Number: byte; Scores: Score list }
type OpenGame = { Id: GameId; StartedAt: DateTime; Rules: Rules; Score: Score list list }

type FinishedGame =
  { Id: GameId
    StartedAt: DateTime
    FinishedAt: DateTime
    Rules: Rules
    Score: Score list list }

type Game =
  | Open of OpenGame
  | Finished of FinishedGame

type addPoint = OpenGame -> Team -> DateTime -> Game
let openGame rules startedAt gameId = { Id = gameId; StartedAt = startedAt; Rules = rules; Score = [ [] ] }
