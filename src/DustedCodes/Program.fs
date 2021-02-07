namespace DustedCodes

module Program =
    open System
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Caching.Distributed
    open Giraffe
    open Giraffe.EndpointRouting
    open Google.Cloud.Datastore.V1
    open Google.Cloud.PubSub.V1
    open Google.Cloud.Diagnostics.AspNetCore
    open Google.Cloud.Diagnostics.Common

    let mutable private pubSubClient : PublisherClient = null

    let configureServices (settings : Config.Settings) =
        fun (services : IServiceCollection) ->

            let topicName =
                TopicName(
                    settings.GCP.ProjectId,
                    settings.Mail.GcpPubSubTopic)
            pubSubClient <- PublisherClient.CreateAsync(topicName).Result
            let dsClient = DatastoreDb.Create settings.GCP.ProjectId

            let saveEntityFunc =
                Datastore.saveEntity
                        settings.General.AppName
                        settings.General.EnvName
                        settings.Mail.GcpDatastoreKind
                        dsClient
            let publishMsgFunc =
                PubSub.sendMessage
                    settings.General.EnvName
                    settings.Mail.Domain
                    settings.Mail.Sender
                    settings.Mail.Recipient
                    pubSubClient

            let getReportFunc =
                GoogleAnalytics.getMostViewedPagesAsync settings.ThirdParties.AnalyticsKey

            services
                .AddHttpClient(Http.clientName)
                .AddOutgoingGoogleTraceHandler().Services
                .AddGoogleTrace(fun x -> x.ProjectId <- settings.GCP.ProjectId)
                .AddSingleton(settings)
                .AddSingleton<GoogleAnalytics.GetReportFunc>(getReportFunc)
                .AddSingleton<Messages.SaveFunc>(Messages.save saveEntityFunc publishMsgFunc)
                .When(
                    settings.Redis.Enabled,
                    fun svc -> svc.AddRedisCache(settings.Redis.Configuration, settings.Redis.Instance))
                .When(
                    not settings.Redis.Enabled,
                    fun svc -> svc.AddSingleton<IDistributedCache, MemoryDistributedCache>())
                .AddResponseCompression()
                .AddRouting()
                .AddGiraffe()
            |> ignore

    let configureApp (settings : Config.Settings) =
        fun (app : IApplicationBuilder) ->
            app.UseErrorHandler()
               .UseGoogleTrace()
               .UseGoogleRequestLogging(settings.Web.RequestLogging)
               .UseRealIPAddress(settings.Proxy.FwdIPHeaderName, settings.Proxy.ProxyCount)
               .UseTrailingSlashRedirection(settings.Https.HttpsPort)
               .UseHttpsRedirection(
                   settings.Https.ForceHttps,
                   settings.Https.HttpsHost,
                   settings.Https.HttpsPort,
                   settings.Https.BehindProxy,
                   settings.Https.PermanentRedirect)
               .UseStaticFiles()
               .UseResponseCompression()
               .UseRouting()
               .UseGiraffe(Router.endpoints settings)
               .UseGiraffe(HttpHandlers.notFound)

    [<EntryPoint>]
    let main args =
        try
            let log =
                Log.write
                    Log.consoleFormat
                    []
                    Level.Debug
                    None
                    ""
                    ""
            try
                DotEnv.load log
                let settings = Config.loadSettings()
                log Level.Debug (settings.ToString())

                let blogPosts = BlogPosts.load Config.blogPostsPath
                let lastBlogPost =
                    blogPosts
                    |> List.sortByDescending (fun t -> t.PublishDate)
                    |> List.head

                log Level.Info (sprintf "Parsed %i blog posts." blogPosts.Length)
                log Level.Info (sprintf "Last blog post is: %s." lastBlogPost.Title)

                BlogPosts.all <- blogPosts

                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHost(
                        fun webHostBuilder ->
                            webHostBuilder
                                .ConfigureSentry(
                                    settings.ThirdParties.SentryDsn,
                                    settings.General.AppName,
                                    settings.General.AppVersion)
                                .UseKestrel(
                                    fun k -> k.AddServerHeader <- false)
                                .UseContentRoot(Config.appRoot)
                                .UseWebRoot(Config.assetsPath)
                                .Configure(configureApp settings)
                                .ConfigureServices(configureServices settings)
                                |> ignore)
                    .Build()
                    .Run()
                0
            with ex ->
                log Level.Emergency
                    (sprintf "Host terminated unexpectedly: %s\n\nStacktrace: %s" ex.Message ex.StackTrace)
                1
        finally
            if isNotNull pubSubClient
            then pubSubClient.ShutdownAsync(TimeSpan.FromSeconds 10.0).Wait()