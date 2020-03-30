module DustedCodes.Program

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Serilog
open Serilog.Events
open Giraffe

[<EntryPoint>]
let main args =
    let parseLogLevel =
        function
        | "verbose" -> LogEventLevel.Verbose
        | "debug"   -> LogEventLevel.Debug
        | "info"    -> LogEventLevel.Information
        | "warning" -> LogEventLevel.Warning
        | "error"   -> LogEventLevel.Error
        | "fatal"   -> LogEventLevel.Fatal
        | _         -> LogEventLevel.Warning

    let logLevelConsole =
        match isNotNull args && args.Length > 0 with
        | true  -> args.[0]
        | false -> Config.logLevelConsole
        |> parseLogLevel

    Log.Logger <-
        (LoggerConfiguration())
            .MinimumLevel.Information()
            .Enrich.WithProperty("Environment", Config.environmentName)
            .Enrich.WithProperty("Application", "DustedCodes")
            .WriteTo.Console(logLevelConsole)
            .CreateLogger()

    try
        try
            let lastBlogPost =
                BlogPosts.all
                |> List.sortByDescending (fun t -> t.PublishDate)
                |> List.head

            Log.Information "Starting Dusted Codes Blog..."
            Log.Information (sprintf "Parsed %i blog posts." BlogPosts.all.Length)
            Log.Information (sprintf "Last blog post is: %s." lastBlogPost.Title)

            WebHostBuilder()
                .UseSerilog()
                .UseKestrel(fun k -> k.AddServerHeader <- false)
                .UseContentRoot(Config.contentRoot)
                .UseWebRoot(Config.webRoot)
                .Configure(Action<IApplicationBuilder> Web.configureApp)
                .ConfigureServices(Web.configureServices)
                .Build()
                .Run()
            0
        with ex ->
            Log.Fatal(ex, "Host terminated unexpectedly.")
            1
    finally
        Log.CloseAndFlush()