namespace Foosball.Api

open Microsoft.Extensions.Configuration

module App =

  open System
  open System.IO
  open Microsoft.AspNetCore.Builder
  open Microsoft.AspNetCore.Hosting
  open Microsoft.Extensions.Hosting
  open Microsoft.Extensions.Logging
  open Microsoft.Extensions.DependencyInjection
  open Giraffe
  open Settings

  let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

  let configureApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

    (match env.EnvironmentName with
     | "Development" -> app.UseDeveloperExceptionPage()
     | _ -> app.UseGiraffeErrorHandler(errorHandler))
      .UseHttpsRedirection()
      .UseStaticFiles()
      .UseGiraffe(HttpHandler.router)

  let configureServices (services: IServiceCollection) = services.AddGiraffe() |> ignore

  let configureLogging (builder: ILoggingBuilder) =
    builder
      .AddFilter(fun l -> l.Equals LogLevel.Error)
      .AddConsole()
      .AddDebug()
    |> ignore

  let configureSettings (configurationBuilder: IConfigurationBuilder) =
    configurationBuilder
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.json", false)

  [<EntryPoint>]
  let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")
    let confBuilder = ConfigurationBuilder() |> configureSettings
    let settings = (confBuilder.Build().Get<Settings>())

    Host
      .CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(fun webHostBuilder ->
        webHostBuilder
          .UseContentRoot(contentRoot)
          .UseWebRoot(webRoot)
          .Configure(Action<IApplicationBuilder> configureApp)
          .ConfigureServices(configureServices)
          .ConfigureLogging(configureLogging)
        |> ignore)
      .Build()
      .Run()

    0
