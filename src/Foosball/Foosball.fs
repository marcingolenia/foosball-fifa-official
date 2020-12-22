module Foosball1

open System

type GameId = bigint
type Footballers = | Yellow | Black // Official championship's colors
type Rules =
    { PointsInSet: byte
      SetsToWinToBeWinner: byte }

type TeamName = TeamName of string
type Team = TeamName * Footballers
type Score = { By: Team; At: DateTime }
type SetScores = { Number: byte; Scores: Score list }

type OpenGame =
    { Id: GameId
      Teams: Team * Team
      StartedAt: DateTime
      Rules: Rules
      Score: SetScores list }

type FinishedGame =
    { Id: GameId
      Teams: Team * Team
      StartedAt: DateTime
      FinishedAt: DateTime
      Rules: Rules
      Score: SetScores list }

type Game =
    | Open of OpenGame
    | Finished of FinishedGame

type addPoint = OpenGame -> Team -> DateTime -> Game
type openGame = Team * Team -> DateTime -> GameId -> OpenGame

