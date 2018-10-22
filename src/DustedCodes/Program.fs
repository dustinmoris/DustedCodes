module DustedCodes.Program

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Serilog
open Giraffe

let parseLogLevel (lvl : string) =
    match lvl with
    | "verbose" -> Serilog.Events.LogEventLevel.Verbose
    | "debug"   -> Serilog.Events.LogEventLevel.Debug
    | "info"    -> Serilog.Events.LogEventLevel.Information
    | "warning" -> Serilog.Events.LogEventLevel.Warning
    | "fatal"   -> Serilog.Events.LogEventLevel.Fatal
    | _         -> Serilog.Events.LogEventLevel.Error

[<EntryPoint>]
let main args =
    let logLevel =
        match isNotNull args && args.Length > 0 && not (String.IsNullOrEmpty args.[0]) with
        | true  -> args.[0]
        | false -> Config.logLevel
        |> parseLogLevel

    Log.Logger <-
        (new LoggerConfiguration())
            .MinimumLevel.Is(logLevel)
            .WriteTo.Console()
            .CreateLogger()
    try
        try
            let lastBlogPost =
                Web.blogPosts
                |> List.sortByDescending (fun t -> t.PublishDate)
                |> List.head

            Log.Information "Starting Dusted Codes Blog..."

            Log.Information (sprintf "Parsed %i blog posts." Web.blogPosts.Length)
            Log.Information (sprintf "Last blog post is: %s." lastBlogPost.Title)

            WebHostBuilder()
                .UseSerilog()
                .UseKestrel()
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