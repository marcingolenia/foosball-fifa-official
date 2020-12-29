namespace Foosball.PostgresPersistence

open System
open Foosball.Application

module DbModels =
  type GameDetails =
    { Id: int64
      MaxSets: int
      MaxSetPoints: int
      StartedAt: DateTime
      FinishedAt: DateTime option
      Team1: string
      Team2: string }
    member this.toQueryResult scores: Queries.GameDetails =
      { Id = this.Id
        MaxSets = this.MaxSets
        MaxSetPoints = this.MaxSetPoints
        StartedAt = this.StartedAt
        FinishedAt = this.FinishedAt
        Team1 = this.Team1
        Team2 = this.Team2
        Score = scores }
