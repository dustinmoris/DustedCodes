namespace DustedCodes

module Program =
    open System
    open System.Collections.Generic
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Caching.Distributed
    open Giraffe
    open Giraffe.EndpointRouting
    open Logfella
    open Logfella.LogWriters
    open Logfella.Adapters
    open Logfella.AspNetCore

    let private muteFilter =
        Func<Severity, string, IDictionary<string, obj>, exn, bool>(
            fun severity msg data ex ->
                msg.StartsWith "The response could not be cached for this request")

    let private createLogWriter (ctx : HttpContext option) =
        match Env.isProduction with
        | false -> ConsoleLogWriter(Env.logSeverity).AsLogWriter()
        | true  ->
            let basic =
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
            let final =
                match ctx with
                | None     -> basic
                | Some ctx ->
                    basic
                        .AddHttpContext(ctx)
                        .AddCorrelationId(Guid.NewGuid().ToString("N"))
            Mute.When(muteFilter)
                .Otherwise(final)

    let private createReqLogWriter =
        Func<HttpContext, ILogWriter>(Some >> createLogWriter)

    let private toggleRequestLogging =
        Action<RequestLoggingOptions>(
            fun x -> x.IsEnabled <- Env.enableRequestLogging)

    let configureServices (services : IServiceCollection) =
        match Env.redisEnabled with
        | true ->
            services.AddStackExchangeRedisCache(
                fun o ->
                    o.InstanceName  <- Env.redisInstance
                    o.Configuration <- Env.redisConfiguration)
        | false ->
            services.AddSingleton<IDistributedCache, MemoryDistributedCache>()
        |> ignore

        services
            .AddProxies(
                Env.proxyCount,
                Env.knownProxyNetworks,
                Env.knownProxies)
            .AddResponseCaching()
            .AddResponseCompression()
            .AddRouting()
            .AddGiraffe()
        |> ignore

    let configureApp (app : IApplicationBuilder) =
        app.UseGiraffeErrorHandler(WebApp.errorHandler)
           .When(Env.enableTracing, Middlewares.logResponseTime)
           .UseRequestScopedLogWriter(createReqLogWriter)
           .UseGiraffeErrorHandler(WebApp.errorHandler)
           .UseRequestLogging(toggleRequestLogging)
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
            Log.SetDefaultLogWriter(createLogWriter None)
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