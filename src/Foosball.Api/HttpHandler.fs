namespace Foosball.Api

module HttpHandler =
  
  open Microsoft.AspNetCore.Hosting
  open Microsoft.AspNetCore.Http
  open Giraffe

  let router: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ GET >=> route "/" >=> htmlView Views.index
             GET >=> route "/games" >=> htmlView Views.index
             POST >=> route "/games" >=> htmlView Views.index
             GET >=> route "/games/%d" >=> htmlView Views.index
             POST >=> route "/games/scores" >=> htmlView Views.index
             setStatusCode 404 >=> text "Not Found" ]
