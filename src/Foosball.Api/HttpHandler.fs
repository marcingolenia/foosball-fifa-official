namespace Foosball.Api

module HttpHandler =
  open Microsoft.AspNetCore.Hosting
  open Microsoft.AspNetCore.Http
  open Foosball.Api.CompositionRoot.Root
  open Giraffe
  open FSharp.Control.Tasks.V2.ContextInsensitive
  open Foosball.Application.Queries

  let private scoreLink finishedDate id =
    match finishedDate with
    | Some _ -> []
    | None -> [ { Rel = "score"; Href = $"/games/{id}/score" } ]

  let private detailsLink id = { Rel = "self"; Href = $"/games/{id}" }

  let readGameByHandler readGameBy (id: int64): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let! game = readGameBy id
        let links = scoreLink game.FinishedAt id @ [{ Rel = "all"; Href = "/games" } ]
        let response = { Result = game; Links = links }
        return! json response next ctx
      }

  let listGamesHandler listGames: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let! (games: GameOverview seq) = listGames
        let result = games |> Seq.map (fun game ->
                     { Result = game
                       Links = scoreLink game.FinishedAt game.Id @ [ detailsLink game.Id ] })
        return! json result next ctx
      }

  let scoreHandler makeScore (id: int64): HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      task {
        let! scoreDto = ctx.BindJsonAsync<ScoreDto>()
        let! scoreResult = makeScore id scoreDto.Color scoreDto.Team
        let response =
          match scoreResult with
          | Ok _ ->
              let links = [ { Rel = "up"; Href = $"/games/{id}" } ]
              Successful.created (json links) next ctx
          | Error message -> RequestErrors.badRequest (json message) next ctx
        return! response
      }

  let createGameHandler createGame generateId: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
      let newId = generateId ()
      task {
        let! newGameDto = ctx.BindJsonAsync<NewGameDto>()
        let! creationResult = createGame newId newGameDto.Team1 newGameDto.Team2
        let response =
          match creationResult with
          | Ok _ ->
              ctx.SetHttpHeader "Location" (sprintf "/games/%d" newId)
              let links = [ (detailsLink newId); { Rel = "all"; Href = "/games" } ]
              Successful.created (json links) next ctx
          | Error message -> RequestErrors.badRequest (json message) next ctx
        return! response
      }

  let notFoundHandler: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> text $"{ctx.Request.Path.Value} was not found" next ctx

  let router (root: Root): HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ GET >=> route "/" >=> htmlView Views.index
             GET >=> route "/games" >=> (listGamesHandler root.ListGames)
             POST >=> route "/games" >=> (createGameHandler root.CreateGame root.GenerateId)
             GET >=> routef "/games/%d" (readGameByHandler root.ReadGameBy)
             POST >=> routef "/games/%d/scores" (scoreHandler root.Score)
             setStatusCode 404 >=> notFoundHandler ]
