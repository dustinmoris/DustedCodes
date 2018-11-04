module DustedCodes.Program

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Serilog
open Serilog.Events
open Serilog.Sinks.Elasticsearch
open Elasticsearch.Net
open Giraffe

[<EntryPoint>]
let main args =
    let logLevel =
        match isNotNull args && args.Length > 0 with
        | true  -> args.[0]
        | false -> Config.logLevel
        |> (function
            | "verbose" -> LogEventLevel.Verbose
            | "debug"   -> LogEventLevel.Debug
            | "info"    -> LogEventLevel.Information
            | "warning" -> LogEventLevel.Warning
            | "error"   -> LogEventLevel.Error
            | "fatal"   -> LogEventLevel.Fatal
            | _         -> LogEventLevel.Warning)

    let elasticOptions =
        new ElasticsearchSinkOptions(
            new Uri(Config.elasticUrl),
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
            ModifyConnectionSettings =
                fun (config : ConnectionConfiguration) ->
                    config.BasicAuthentication(
                        Config.elasticUser,
                        Config.elasticPassword))

    Log.Logger <-
        (new LoggerConfiguration())
            .MinimumLevel.Is(logLevel)
            .Enrich.WithProperty("Environment", Config.environmentName)
            .Enrich.WithProperty("Application", "DustedCodes")
            .WriteTo.Console()
            .WriteTo.Elasticsearch(elasticOptions)
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