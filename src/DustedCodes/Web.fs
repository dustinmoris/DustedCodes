namespace DustedCodes

[<RequireQualifiedAccess>]
module HttpHandlers =
    open System
    open System.Linq
    open System.Text
    open System.Threading.Tasks
    open System.Diagnostics
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Http.Extensions
    open Microsoft.Extensions.Caching.Distributed
    open Microsoft.Net.Http.Headers
    open FSharp.Control.Tasks
    open Giraffe
    open Giraffe.ViewEngine
    open Logfella

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
            let encodedUrl = ctx.Request.GetEncodedUrl()
            Log.Notice(
                sprintf "Could not serve '%s %s', because it does not exist."
                    ctx.Request.Method
                    encodedUrl,
                dict [
                    "httpError", "404" :> obj
                    "httpVerb", ctx.Request.Method :> obj
                    "encodedUrl", encodedUrl :> obj
                ])
            (setStatusCode 404
            >=> htmlView Views.notFound) next ctx

    let index =
        allowCaching (TimeSpan.FromHours 1.0)
        >=> (
            BlogPosts.all
            |> List.sortByDescending (fun p -> p.PublishDate)
            |> Views.index
            |> htmlView)

    let pingPong : HttpHandler =
        noResponseCaching >=> text "pong"

    let version : HttpHandler =
        noResponseCaching
        >=> json {| version = Env.appVersion |}

    let about =
        allowCaching (TimeSpan.FromDays 1.0)
        >=> (Views.about |> htmlView)

    let hire =
        Views.hire ContactMessages.Entity.Empty None
        |> htmlView

    let contact (next : HttpFunc) =
        let internalErr =
            "Unfortunately the message failed to send due to an unexpected error. "
            + "The website owner has been notified about the issue. "
            + "Please try a little bit later again."
            |> Error
        fun (ctx : HttpContext) ->
            task {
                let respond msg res =
                    htmlView (Views.hire msg (Some res)) next ctx

                let! msg = ctx.BindFormAsync<ContactMessages.Entity>()
                match msg.IsValid with
                | Error err -> return! respond msg (Error err)
                | Ok _      ->
                    let! captchaResult =
                        ctx.Request.Form.["h-captcha-response"].ToString()
                        |> Captcha.validate
                            Env.captchaSiteKey
                            Env.captchaSecretKey
                    match captchaResult with
                    | Captcha.ServerError err ->
                        Log.Critical(
                            sprintf "Captcha validation failed with: %s" err,
                            ("captchaError", err :> obj))
                        return! respond msg internalErr
                    | Captcha.UserError err -> return! respond msg (Error err)
                    | Captcha.Success ->
                        let timer = Stopwatch.StartNew()
                        let dataTask  = DataService.saveContactMessage msg
                        let emailTask = EmailService.sendContactMessage msg
                        do! Task.WhenAll(dataTask, emailTask)
                        timer.Stop()
                        Log.Debug(sprintf "Sending message completed in %fms" timer.Elapsed.TotalMilliseconds)
                        match dataTask.Result, emailTask.Result with
                        | Ok _, _ | _, Ok _ ->
                            return! respond ContactMessages.Entity.Empty (Ok "Thank you, your message has been successfully sent!")
                        | _ -> return! respond msg internalErr
            }

    let blogPost (id : string) =
        BlogPosts.all
        |> List.tryFind (fun x -> Str.equalsCi x.Id id)
        |> function
            | None -> notFound
            | Some blogPost ->
                let eTag = EntityTagHeaderValue.FromString false blogPost.HashCode
                validatePreconditions (Some eTag) None
                >=> allowCaching (TimeSpan.FromDays 7.0)
                >=> (blogPost |> Views.blogPost |> htmlView)

    let trending : HttpHandler =
        let pageMatchesBlogPost =
            fun (pageStat : GoogleAnalytics.PageStatistic) (blogPost : BlogPosts.Article) ->
                pageStat.Path.ToLower().Contains(blogPost.Id.ToLower())

        let pageIsBlogPost (pageStat : GoogleAnalytics.PageStatistic) =
            BlogPosts.all |> List.exists (pageMatchesBlogPost pageStat)

        allowCaching (TimeSpan.FromDays 5.0)
        >=> fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let cacheKey = "trendingBlogPosts"
                let cache = ctx.GetService<IDistributedCache>()

                let! cacheItem = cache.GetAsync(cacheKey, ctx.RequestAborted)
                match Option.ofObj cacheItem with
                | Some view -> return! htmlBytes view next ctx
                | None ->
                    let! mostViewedPages =
                        GoogleAnalytics.getMostViewedPagesAsync
                            Env.googleAnalyticsKey
                            Env.googleAnalyticsViewId
                            (int Byte.MaxValue)
                    let view =
                        mostViewedPages
                        |> List.filter pageIsBlogPost
                        |> List.sortByDescending (fun p -> p.ViewCount)
                        |> List.take 10
                        |> List.map (fun p -> BlogPosts.all.First(fun b -> pageMatchesBlogPost p b))
                        |> Views.trending
                        |> RenderView.AsBytes.htmlDocument

                    do! cache.SetAsync(cacheKey, view, ctx.RequestAborted)

                    return! htmlBytes view next ctx
            }

    let tagged (tag : string) =
        allowCaching (TimeSpan.FromDays 5.0)
        >=>(
            BlogPosts.all
            |> List.filter (fun p -> p.Tags.IsSome && p.Tags.Value.Contains tag)
            |> List.sortByDescending (fun p -> p.PublishDate)
            |> Views.tagged tag
            |> htmlView)

    let rssFeed : HttpHandler =
        let rssFeed = StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
        BlogPosts.all
        |> List.sortByDescending (fun p -> p.PublishDate)
        |> List.take 10
        |> List.map (
            fun b ->
                Feed.Item.Create
                    b.Permalink
                    b.Title
                    b.HtmlContent
                    b.PublishDate
                    Env.blogAuthor
                    Url.``/feed/rss``
                    b.Tags)
        |> Feed.Channel.Create
            Url.``/feed/rss``
            Env.blogTitle
            Env.blogSubtitle
            Env.blogLanguage
            "Giraffe (https://github.com/giraffe-fsharp/Giraffe)"
        |> RssFeed.create
        |> RenderView.IntoStringBuilder.xmlNode rssFeed

        allowCaching (TimeSpan.FromDays 1.0)
        >=> setHttpHeader "Content-Type" "application/rss+xml"
        >=> (rssFeed.ToString() |> setBodyFromString)

    let atomFeed : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            Log.Warning "Someone tried to subscribe to the Atom feed."
            ServerErrors.notImplemented (text "Atom feed is not supported at the moment. If you were using Atom to subscribe to this blog before, please file an issue on https://github.com/dustinmoris/DustedCodes to create awareness.") next ctx

[<RequireQualifiedAccess>]
module WebApp =
    open System
    open Microsoft.Extensions.Logging
    open Giraffe
    open Giraffe.EndpointRouting

    let endpoints : Endpoint list =
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
                if Env.enableErrorEndpoint then
                    route UrlPaths.Debug.``/error`` (warbler (fun _ -> json(1/0)))

                // Content paths
                route UrlPaths.``/``          HttpHandlers.index
                route UrlPaths.``/about``     HttpHandlers.about
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

    let errorHandler (ex : Exception) (logger : ILogger) =
        // Must use the Microsoft.Extensions.Logging.ILogger, because the Sentry
        // integration hooks into the Microsoft logging framework:
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse
        >=> setStatusCode 500
        >=> (match Env.isProduction with
            | false -> Some ex.Message
            | true  -> None
            |> Views.internalError
            |> htmlView)