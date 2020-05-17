namespace DustedCodes

[<RequireQualifiedAccess>]
module HttpHandlers =
    open System
    open System.Text
    open System.Linq
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Http.Extensions
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.Caching.Memory
    open Microsoft.Net.Http.Headers
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open Giraffe.GiraffeViewEngine

    // ---------------------------------
    // Http Handlers
    // ---------------------------------

    let private allowCaching (duration : TimeSpan) : HttpHandler =
        publicResponseCaching (int duration.TotalSeconds) (Some "Accept-Encoding")

    let private svgHandler (svg : XmlNode) : HttpHandler =
        allowCaching (TimeSpan.FromDays 30.0)
        >=> setHttpHeader "Content-Type" "image/svg+xml"
        >=> setBodyFromString (svg |> renderXmlNode)

    let logo =
        svgHandler Icons.logo

    let css : HttpHandler =
        let eTag = EntityTagHeaderValue.FromString false Views.minifiedCss.Hash
        validatePreconditions (Some eTag) None
        >=> allowCaching (TimeSpan.FromDays 365.0)
        >=> setHttpHeader "Content-Type" "text/css"
        >=> setBodyFromString Views.minifiedCss.Content

    let notFound =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let logger = ctx.GetLogger("NotFoundHandler")
            logger.LogWarning(
                "Could not serve '{verb} {url}', because it does not exist.",
                ctx.Request.Method,
                ctx.Request.GetEncodedUrl())
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
        None |> Views.hire |> htmlView

    let private contactResponse next ctx result =
        result
        |> Some
        |> Views.hire
        |> htmlView
        <|| (next, ctx)

    let private contactError next ctx contactMsg errorMsg =
        Error (contactMsg, errorMsg)
        |> contactResponse next ctx

    let private contactSuccess next ctx =
        Ok "Thank you, your message has been successfully sent!"
        |> contactResponse next ctx

    let contact =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! message = ctx.BindFormAsync<ContactMessage>()
                match message.ValidationResult with
                | Error err -> return! contactError next ctx message err
                | Ok _      ->
                    let! captchaResult =
                        ctx.Request.Form.["g-recaptcha-response"].ToString()
                        |> Captcha.validateAsync Env.googleCaptchaSecretKey
                    match captchaResult with
                    | Captcha.ServerError err ->
                        let logger = ctx.GetLogger()
                        logger.LogError("Captcha validation failed with '{captchaError}'.", err)
                        return! contactError next ctx message "Verification failed. Please try again."
                    | Captcha.UserError err -> return! contactError next ctx message err
                    | Captcha.Success ->
                        match! DataService.saveContactMessageAsync message with
                        | Error err ->
                            let logger = ctx.GetLogger()
                            logger.LogError("An error occurred when writing to the datastore: '{dataError}'.", err)
                            return! contactError next ctx message "An unexpected error occurred. Please try again."
                        | Ok _      -> return! contactSuccess next ctx
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
                let cache = ctx.GetService<IMemoryCache>()

                match cache.TryGetValue cacheKey with
                | true, view -> return! htmlView (view :?> GiraffeViewEngine.XmlNode) next ctx
                | false, _   ->
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

                    // Cache the view for one day
                    let cacheOptions = MemoryCacheEntryOptions()
                    cacheOptions.SetAbsoluteExpiration(DateTimeOffset.Now.AddDays(1.0)) |> ignore
                    cache.Set(cacheKey, view, cacheOptions) |> ignore

                    return! htmlView view next ctx
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
        |> ViewBuilder.buildXmlNode rssFeed

        allowCaching (TimeSpan.FromDays 1.0)
        >=> setHttpHeader "Content-Type" "application/rss+xml"
        >=> (rssFeed.ToString() |> setBodyFromString)

    let atomFeed : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let logger = ctx.GetLogger("AtomFeedHandler")
            logger.LogWarning "Someone tried to subscribe to the Atom feed."
            ServerErrors.notImplemented (text "Atom feed is not supported at the moment. If you were using Atom to subscribe to this blog before, please file an issue on https://github.com/dustinmoris/DustedCodes to create awareness.") next ctx

[<RequireQualifiedAccess>]
module WebApp =
    open System
    open Microsoft.Extensions.Logging
    open Giraffe

    let routes : HttpHandler =
        choose [
            GET_HEAD >=>
                choose [
                    // Static assets
                    route  UrlPaths.``/logo.svg``   >=> HttpHandlers.logo
                    routef "/bundle.%s.css"         (fun _ -> HttpHandlers.css)

                    // Health check
                    route UrlPaths.``/ping``        >=> HttpHandlers.pingPong
                    route UrlPaths.``/version``     >=> HttpHandlers.version

                    // Debug
                    if Env.enableErrorEndpoint then
                        route UrlPaths.Debug.``/error`` >=> warbler (fun _ -> json(1/0))

                    // Content paths
                    route    UrlPaths.``/``         >=> HttpHandlers.index
                    routeCi  UrlPaths.``/about``    >=> HttpHandlers.about
                    routeCi  UrlPaths.``/hire``     >=> HttpHandlers.hire
                    routeCi  UrlPaths.``/trending`` >=> HttpHandlers.trending

                    routeCi UrlPaths.``/feed/rss``  >=> HttpHandlers.rssFeed
                    routeCi UrlPaths.``/feed/atom`` >=> HttpHandlers.atomFeed

                    // Deprecated URLs kept alive in order to not break
                    // existing links in the world wide web
                    routeCi  UrlPaths.Deprecated.``/archive`` >=> HttpHandlers.index

                    routeCif UrlPaths.``/tagged/%s`` HttpHandlers.tagged
                    routeCif UrlPaths.``/%s`` HttpHandlers.blogPost
                ]
            POST >=> routeCi UrlPaths.``/hire`` >=> HttpHandlers.contact
            HttpHandlers.notFound ]

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