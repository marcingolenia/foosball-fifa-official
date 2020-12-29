namespace Foosball.Application

open System

module Queries =

  type Score = { SetNo: int; Team: string; ScoredWith: string; ScoredAt: DateTime }

  type GameOverview =
    { Id: int64
      StartedAt: DateTime
      FinishedAt: DateTime option
      Team1: string
      Team2: string
      Team1Score: int
      Team2Score: int }

  type GameDetails =
    { Id: int64
      MaxSets: int
      MaxSetPoints: int
      StartedAt: DateTime
      FinishedAt: DateTime option
      Team1: string
      Team2: string
      Score: Score list }
