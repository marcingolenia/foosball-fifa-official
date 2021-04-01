module Acceptance.UsingTestServer.TestApi

open System
open System.Net.Http
open Foosball.Api
open Foosball.Api.CompositionRoot
open Microsoft.Extensions.Configuration
open Settings
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Newtonsoft.Json

let selfHosted =
  let confBuilder = ConfigurationBuilder() |> App.configureSettings
  let root = (confBuilder.Build().Get<Settings>()) |> Trunk.compose |> Root.compose
  WebHostBuilder()
    .UseTestServer()
    .Configure(Action<IApplicationBuilder>(App.configureApp root))
    .ConfigureServices(App.configureServices)
    .ConfigureLogging(App.configureLogging)

type HttpContent with
  static member ToString (httpContent: HttpContent) =
    httpContent.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously

type HttpClient with
  member this.PostGame(game: NewGameDto) =
    let json = JsonConvert.SerializeObject game
    use content = new StringContent(json, Text.Encoding.UTF8, "application/json")
    this.PostAsync("games", content) |> Async.AwaitTask |> Async.RunSynchronously
