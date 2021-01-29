namespace DustedCodes

module Program =
    open System
    open System.Collections.Generic
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Giraffe
    open Giraffe.EndpointRouting
    open Logfella
    open Logfella.LogWriters
    open Logfella.Adapters
    open Logfella.AspNetCore

    let private googleCloudLogWriter =
        GoogleCloudLogWriter
            .Create(Env.logSeverity)
            .AddServiceContext(
                Env.appName,
                Env.appVersion)
            .UseGoogleCloudTimestamp()
            .AddLabels(
                dict [
                    "appName", Env.appName
                    "appVersion", Env.appVersion
                ])

    let private muteFilter =
        Func<Severity, string, IDictionary<string, obj>, exn, bool>(
            fun severity msg data ex ->
                msg.StartsWith "The response could not be cached for this request")

    let private defaultLogWriter =
        Mute.When(muteFilter)
            .Otherwise(
                match Env.isProduction with
                | false -> ConsoleLogWriter(Env.logSeverity).AsLogWriter()
                | true  -> googleCloudLogWriter.AsLogWriter())

    let configureServices (services : IServiceCollection) =
        services
            .AddProxies(
                Env.proxyCount,
                Env.knownProxyNetworks,
                Env.knownProxies)
            .AddMemoryCache()
            .AddResponseCaching()
            .AddResponseCompression()
            .AddRouting()
            .AddGiraffe()
        |> ignore

    let configureApp (app : IApplicationBuilder) =
        app.UseGiraffeErrorHandler(WebApp.errorHandler)
           .UseWhen(
                (fun _ -> Env.isProduction),
                fun x ->
                    x.UseRequestScopedLogWriter(
                        fun ctx ->
                            Mute.When(muteFilter)
                                .Otherwise(
                                    googleCloudLogWriter
                                        .AddHttpContext(ctx)
                                        .AddCorrelationId(Guid.NewGuid().ToString("N"))
                                        .AsLogWriter()))
                    |> ignore)
           .UseGiraffeErrorHandler(WebApp.errorHandler)
           .UseRequestLogging(
                fun cfg ->
                    cfg.IsEnabled    <- Env.enableRequestLogging
                    cfg.LogOnlyAfter <- false)
           .UseForwardedHeaders()
           .UseHttpsRedirection(Env.forceHttps, Env.domainName)
           .UseTrailingSlashRedirection()
           .UseStaticFiles()
           .UseResponseCaching()
           .UseResponseCompression()
           .UseRouting()
           .UseGiraffe(WebApp.endpoints)
           .UseGiraffe(HttpHandlers.notFound)
           |> ignore

    [<EntryPoint>]
    let main _ =
        try
            Log.SetDefaultLogWriter(defaultLogWriter)
            Logging.outputEnvironmentSummary Env.summary

            let lastBlogPost =
                BlogPosts.all
                |> List.sortByDescending (fun t -> t.PublishDate)
                |> List.head

            Log.Info (sprintf "Parsed %i blog posts." BlogPosts.all.Length)
            Log.Info (sprintf "Last blog post is: %s." lastBlogPost.Title)

            Host.CreateDefaultBuilder()
                .UseLogfella()
                .ConfigureWebHost(
                    fun webHostBuilder ->
                        webHostBuilder
                            .ConfigureSentry(
                                Env.sentryDsn,
                                Env.name,
                                Env.appVersion)
                            .UseKestrel(
                                fun k -> k.AddServerHeader <- false)
                            .UseContentRoot(Env.appRoot)
                            .UseWebRoot(Env.assetsDir)
                            .Configure(configureApp)
                            .ConfigureServices(configureServices)
                            |> ignore)
                .Build()
                .Run()
            0
        with ex ->
            Log.Emergency("Host terminated unexpectedly.", ex)
            1