namespace DustedCodes

module Program =
    open System
    open System.Net.Http
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Caching.Distributed
    open Giraffe
    open Giraffe.EndpointRouting

    let configureServices (settings : Config.Settings) =
        fun (services : IServiceCollection) ->

            let captchaClient = new HttpClient()
            captchaClient.BaseAddress <-
                Uri("https://hcaptcha.com/siteverify")
            captchaClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "dusted.codes")
            let postCaptcha     = Http.postForm captchaClient
            let validateCaptcha = Captcha.validate postCaptcha

            let mailDropClient = new HttpClient()
            mailDropClient.BaseAddress <-
                Uri(settings.Mail.MailDropEndpoint)
            mailDropClient.DefaultRequestHeaders.Add(
                "Authorization",
                sprintf "Bearer %s" settings.Mail.MailDropApiKey)
            mailDropClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "dusted.codes")
            let postMail = Http.postJson mailDropClient
            let saveMail =
                Messages.save
                    postMail
                    settings.General.EnvName
                    settings.Mail.Domain
                    settings.Mail.Sender
                    settings.Mail.Recipient

            let getReportFunc =
                GoogleAnalytics.getMostViewedPagesAsync settings.ThirdParties.AnalyticsKey

            services
                .AddHttpClient()
                .AddSingleton(settings)
                .AddSingleton<GoogleAnalytics.GetReportFunc>(getReportFunc)
                .AddSingleton<Captcha.ValidateFunc>(validateCaptcha)
                .AddSingleton<Messages.SaveFunc>(saveMail)
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