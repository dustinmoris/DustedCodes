namespace DustedCodes

[<RequireQualifiedAccess>]
module HttpHandlers =
    open System
    open System.Linq
    open System.IO
    open System.Text
    open System.Diagnostics
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Caching.Distributed
    open Microsoft.Net.Http.Headers
    open FSharp.Control.Tasks
    open Giraffe
    open Giraffe.ViewEngine

    // ---------------------------------
    // Http Handlers
    // ---------------------------------

    let private htmlBytes (htmlView : byte array) : HttpHandler =
        fun (_ : HttpFunc) (ctx : HttpContext) ->
            ctx.SetContentType "text/html; charset=utf-8"
            ctx.WriteBytesAsync htmlView

    let private allowCaching (duration : TimeSpan) : HttpHandler =
        publicResponseCaching (int duration.TotalSeconds) (Some "Accept-Encoding")

    let svg (element : XmlNode) : HttpHandler =
        allowCaching (TimeSpan.FromDays 30.0)
        >=> setHttpHeader "Content-Type" "image/svg+xml"
        >=> setBodyFromString (element |> RenderView.AsString.xmlNode)

    let css : HttpHandler =
        let eTag = EntityTagHeaderValue.FromString false Views.minifiedCss.Hash
        validatePreconditions (Some eTag) None
        >=> allowCaching (TimeSpan.FromDays 365.0)
        >=> setHttpHeader "Content-Type" "text/css"
        >=> setBodyFromString Views.minifiedCss.Content

    let notFound =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            let log      = ctx.GetLogFunc()
            log Level.Info (sprintf "Page not found: %s" (ctx.GetRequestUrl()))
            (setStatusCode 404 >=> htmlView (Views.notFound settings)) next ctx

    let index =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            (allowCaching (TimeSpan.FromHours 1.0)
            >=> (
                BlogPosts.all
                |> List.sortByDescending (fun p -> p.PublishDate)
                |> Views.index settings
                |> htmlView)) next ctx

    let pingPong : HttpHandler =
        noResponseCaching >=> text "pong"

    let version (next : HttpFunc) (ctx : HttpContext) =
        let settings = ctx.GetService<Config.Settings>()
        (noResponseCaching
        >=> json {| version = settings.General.AppVersion |}) next ctx

    let markdown fileName (view : string -> XmlNode) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! content =
                    Path.Combine(Config.contentRoot, fileName)
                    |> File.ReadAllTextAsync
                let handler =
                    content
                    |> MarkDog.toHtml
                    |> view
                    |> htmlView
                return!
                    (allowCaching (TimeSpan.FromDays 1.0)
                    >=> handler)
                    next ctx
            }

    let about : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            markdown
                "About.md"
                (Views.about settings)
                next ctx

    let hire : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            markdown
                "Hire.md"
                (Views.hire
                    settings
                    Messages.ContactMsg.Empty
                    None)
                next ctx

    let contact (next : HttpFunc) =
        let internalErr =
            "Unfortunately the message failed to send due to an unexpected error. "
            + "The website owner has been notified about the issue. "
            + "Please try a little bit later again."
            |> Error
        fun (ctx : HttpContext) ->
            task {
                let settings = ctx.GetService<Config.Settings>()
                let log      = ctx.GetLogFunc()

                let respond msg res =
                    markdown
                        "Hire.md"
                        (Views.hire settings msg (Some res))
                        next ctx

                let! msg = ctx.BindFormAsync<Messages.ContactMsg>()
                match msg.IsValid with
                | Error err -> return! respond msg (Error err)
                | Ok _      ->
                    let! captchaResult =
                        ctx.Request.Form.["h-captcha-response"].ToString()
                        |> Captcha.validate
                            log
                            settings.ThirdParties.CaptchaSiteKey
                            settings.ThirdParties.CaptchaSecretKey
                    match captchaResult with
                    | Captcha.ServerError err ->
                        log Level.Critical
                            (sprintf "Captcha validation failed with: %s" err)
                        return! respond msg internalErr
                    | Captcha.UserError err -> return! respond msg (Error err)
                    | Captcha.Success ->
                        let saveMsg = ctx.GetService<Messages.SaveFunc>()
                        let timer = Stopwatch.StartNew()
                        let! result = saveMsg log msg
                        timer.Stop()
                        log Level.Debug (sprintf "Sent message in %s" (timer.Elapsed.ToMs()))
                        match result with
                        | Ok _    -> return! respond Messages.ContactMsg.Empty result
                        | Error _ -> return! respond msg internalErr
            }

    let private getBlogPostFromCache (id : string) =
        BlogPosts.all
        |> List.tryFind (fun x -> x.Id.EqualsCi id)

    let private getBlogPostFromDisk (blogPostRoot : string) (id : string) =
        match BlogPosts.tryFindSinglePost blogPostRoot id with
        | Ok article -> Some article
        | Error _    -> None

    let blogPost (id : string) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            let article =
                match settings.General.IsProd with
                | true  -> getBlogPostFromCache id
                | false -> getBlogPostFromDisk Config.blogPostsPath id
            (match article with
            | None    -> notFound
            | Some bp ->
                let eTag = EntityTagHeaderValue.FromString false bp.HashCode
                validatePreconditions (Some eTag) None
                >=> allowCaching (TimeSpan.FromDays 7.0)
                >=> (bp |> Views.blogPost settings  |> htmlView)) next ctx

    let trending : HttpHandler =
        let pageMatchesBlogPost =
            fun (pageStat : GoogleAnalytics.PageStatistic) (blogPost : BlogPosts.Article) ->
                pageStat.Path.ToLower().Contains(blogPost.Id.ToLower())

        let pageIsBlogPost (pageStat : GoogleAnalytics.PageStatistic) =
            BlogPosts.all |> List.exists (pageMatchesBlogPost pageStat)

        allowCaching (TimeSpan.FromDays 7.0)
        >=> fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let settings = ctx.GetService<Config.Settings>()
                let cacheKey = settings.Redis.CacheKeyTrending
                let cache    = ctx.GetService<IDistributedCache>()
                let log      = ctx.GetService<Log.Func>()

                let! cacheItem = cache.GetAsync(cacheKey, ctx.RequestAborted)
                match Option.ofObj cacheItem with
                | Some view -> return! htmlBytes view next ctx
                | None ->
                    let getReport = ctx.GetService<GoogleAnalytics.GetReportFunc>()
                    let timer = Stopwatch.StartNew()
                    let! mostViewedPages =
                        getReport
                            log
                            settings.ThirdParties.AnalyticsViewId
                            (int Byte.MaxValue)
                    timer.Stop()
                    log Level.Debug (sprintf "Pulled Google Analytics report in %s." (timer.Elapsed.ToMs()))
                    let view =
                        mostViewedPages
                        |> List.filter pageIsBlogPost
                        |> List.sortByDescending (fun p -> p.ViewCount)
                        |> List.take 10
                        |> List.map (fun p -> BlogPosts.all.First(fun b -> pageMatchesBlogPost p b))
                        |> Views.trending settings
                        |> RenderView.AsBytes.htmlDocument

                    let options = DistributedCacheEntryOptions()
                    options.AbsoluteExpiration <- DateTimeOffset.UtcNow.AddDays(7.0)
                    do! cache.SetAsync(cacheKey, view, options, ctx.RequestAborted)

                    return! htmlBytes view next ctx
            }

    let tagged (tag : string) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            (allowCaching (TimeSpan.FromDays 5.0)
            >=>(
                BlogPosts.all
                |> List.filter (fun p -> p.Tags.IsSome && p.Tags.Value.Contains tag)
                |> List.sortByDescending (fun p -> p.PublishDate)
                |> Views.tagged settings tag
                |> htmlView)) next ctx

    let rssFeed =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let settings = ctx.GetService<Config.Settings>()
            let rssFeed = StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
            BlogPosts.all
            |> List.sortByDescending (fun p -> p.PublishDate)
            |> List.take 10
            |> List.map (
                fun b ->
                    Feed.Item.Create
                        (b.Permalink settings.Web.BaseUrl)
                        b.Title
                        b.HtmlContent
                        b.PublishDate
                        settings.Blog.Author
                        (Url.``/feed/rss`` settings.Web.BaseUrl)
                        b.Tags)
            |> Feed.Channel.Create
                (Url.``/feed/rss`` settings.Web.BaseUrl)
                settings.Blog.Title
                settings.Blog.Subtitle
                settings.Blog.Lang
                "Giraffe (https://github.com/giraffe-fsharp/Giraffe)"
            |> RssFeed.create
            |> RenderView.IntoStringBuilder.xmlNode rssFeed

            (allowCaching (TimeSpan.FromDays 1.0)
            >=> setHttpHeader "Content-Type" "application/rss+xml"
            >=> (rssFeed.ToString() |> setBodyFromString)) next ctx

    let atomFeed : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let log = ctx.GetLogFunc()
            log Level.Warning "Someone tried to subscribe to the Atom feed."
            ServerErrors.notImplemented (text "Atom feed is not supported at the moment. If you were using Atom to subscribe to this blog before, please file an issue on https://github.com/dustinmoris/DustedCodes to create awareness.") next ctx

[<RequireQualifiedAccess>]
module WebApp =
    open System
    open Microsoft.Extensions.Logging
    open Giraffe
    open Giraffe.EndpointRouting

    let endpoints (settings : Config.Settings) : Endpoint list =
        [
            GET_HEAD [
                // Static assets
                route  UrlPaths.``/logo.svg``   (HttpHandlers.svg Icons.logo)
                routef "/images/link-%s.svg"    (Icons.link >> HttpHandlers.svg)
                routef "/bundle.%s.css"         (fun _ -> HttpHandlers.css)

                // Health check
                route UrlPaths.``/ping``        HttpHandlers.pingPong
                route UrlPaths.``/version``     HttpHandlers.version

                // Debug
                if settings.Web.ErrorEndpoint then
                    route UrlPaths.Debug.``/error`` (warbler (fun _ -> json(1/0)))

                // Content paths
                route UrlPaths.``/``          HttpHandlers.index
                route UrlPaths.``/about``     (HttpHandlers.markdown "About.md" (Views.about settings))
                route UrlPaths.``/hire``      HttpHandlers.hire
                route UrlPaths.``/trending``  HttpHandlers.trending

                route UrlPaths.``/feed/rss``  HttpHandlers.rssFeed
                route UrlPaths.``/feed/atom`` HttpHandlers.atomFeed

                // Deprecated URLs kept alive in order to not break
                // existing links in the world wide web
                route UrlPaths.Deprecated.``/archive`` HttpHandlers.index

                // Keeping old links still working
                // (From observing 404 errors in GCP)
                route
                    "/demystifying-aspnet-mvc-5-error-pages"
                    (redirectTo true "/demystifying-aspnet-mvc-5-error-pages-and-error-logging")

                routef UrlPaths.``/tagged/%s`` HttpHandlers.tagged
                routef UrlPaths.``/%s`` HttpHandlers.blogPost
            ]
            POST [
                route UrlPaths.``/hire`` HttpHandlers.contact
            ]
        ]

    let errorHandler (settings : Config.Settings) =
        fun (ex : Exception) (logger : ILogger) ->
            // Must use the Microsoft.Extensions.Logging.ILogger, because the Sentry
            // integration hooks into the Microsoft logging framework:
            logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
            clearResponse
            >=> setStatusCode 500
            >=> (match settings.General.IsProd with
                | false -> Some ex.Message
                | true  -> None
                |> Views.internalError settings
                |> htmlView)