module DustedCodes.Web

open System
open System.Text
open System.Linq
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.HttpOverrides
open Microsoft.AspNetCore.Http.Extensions
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.DependencyInjection
open Microsoft.Net.Http.Headers
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open Giraffe.GiraffeViewEngine
open Firewall
open DustedCodes
open DustedCodes.BlogPosts
open DustedCodes.GoogleAnalytics
open DustedCodes.Icons
open DustedCodes.Views

// ---------------------------------
// Web app
// ---------------------------------

let allowCaching (duration : TimeSpan) : HttpHandler =
    publicResponseCaching (int duration.TotalSeconds) (Some "Accept-Encoding")

let svgHandler (svg : XmlNode) : HttpHandler =
    allowCaching (TimeSpan.FromDays 30.0)
    >=> setHttpHeader "Content-Type" "image/svg+xml"
    >=> setBodyFromString (svg |> renderXmlNode)

let cssHandler : HttpHandler =
    let eTag = EntityTagHeaderValue.FromString false minifiedCss.Hash
    validatePreconditions (Some eTag) None
    >=> allowCaching (TimeSpan.FromDays 365.0)
    >=> setHttpHeader "Content-Type" "text/css"
    >=> setBodyFromString minifiedCss.Content

let notFoundHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger("Web")
        logger.LogWarning(
            "Could not serve '{verb} {url}', because it does not exist.",
            ctx.Request.Method,
            ctx.Request.GetEncodedUrl())
        (setStatusCode 404
        >=> htmlView notFoundView) next ctx

let indexHandler =
    allowCaching (TimeSpan.FromHours 1.0)
    >=> (
        BlogPosts.all
        |> List.sortByDescending (fun p -> p.PublishDate)
        |> indexView
        |> htmlView)

let aboutHandler =
    allowCaching (TimeSpan.FromDays 1.0)
    >=> (aboutView |> htmlView)

let blogPostHandler (id : string) =
    BlogPosts.all
    |> List.tryFind (fun x -> Str.equalsCi x.Id id)
    |> function
        | None -> notFoundHandler
        | Some blogPost ->
            let eTag = EntityTagHeaderValue.FromString false blogPost.HashCode
            validatePreconditions (Some eTag) None
            >=> allowCaching (TimeSpan.FromDays 7.0)
            >=> (blogPost |> blogPostView |> htmlView)

let trendingHandler : HttpHandler =
    let pageMatchesBlogPost =
        fun (page : PageViewStatistic) (blogPost : BlogPost) ->
            page.Path.ToLower().Contains(blogPost.Id.ToLower())

    let pageIsBlogPost (page : PageViewStatistic) =
        BlogPosts.all |> List.exists (pageMatchesBlogPost page)

    allowCaching (TimeSpan.FromDays 5.0)
    >=> fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let cacheKey = "trendingBlogPosts"
            let cache = ctx.GetService<IMemoryCache>()

            match cache.TryGetValue cacheKey with
            | true, view -> return! htmlView (view :?> GiraffeViewEngine.XmlNode) next ctx
            | false, _   ->
                let! mostViewedPages =
                    GoogleAnalytics.getMostViewedPages
                        Config.googleApisJsonKey
                        Config.googleAnalyticsViewId
                        (int Byte.MaxValue)
                let view =
                    mostViewedPages
                    |> List.filter pageIsBlogPost
                    |> List.sortByDescending (fun p -> p.ViewCount)
                    |> List.take 10
                    |> List.map (fun p -> BlogPosts.all.First(fun b -> pageMatchesBlogPost p b))
                    |> trendingView

                // Cache the view for one day
                let cacheOptions = MemoryCacheEntryOptions()
                cacheOptions.SetAbsoluteExpiration(DateTimeOffset.Now.AddDays(1.0)) |> ignore
                cache.Set(cacheKey, view, cacheOptions) |> ignore

                return! htmlView view next ctx
        }

let taggedHandler (tag : string) =
    allowCaching (TimeSpan.FromDays 5.0)
    >=>(
        BlogPosts.all
        |> List.filter (fun p -> p.Tags.IsSome && p.Tags.Value.Contains tag)
        |> List.sortByDescending (fun p -> p.PublishDate)
        |> tagView tag
        |> htmlView)

let rssFeedHandler : HttpHandler =
    let rssFeed = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
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
                Config.blogAuthor
                Url.``/feed/rss``
                b.Tags)
    |> Feed.Channel.Create
        Url.``/feed/rss``
        Config.blogTitle
        Config.blogDescription
        Config.blogLanguage
        "Giraffe (https://github.com/giraffe-fsharp/Giraffe)"
    |> RssFeed.create
    |> ViewBuilder.buildXmlNode rssFeed

    allowCaching (TimeSpan.FromDays 1.0)
    >=> setHttpHeader "Content-Type" "application/rss+xml"
    >=> (rssFeed.ToString() |> setBodyFromString)

let atomFeedHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger("Web")
        logger.LogWarning "Someone tried to subscribe to the Atom feed."
        ServerErrors.notImplemented (text "Atom feed is not supported at the moment. If you were using Atom to subscribe to this blog before, please file an issue on https://github.com/dustinmoris/DustedCodes to create awareness.") next ctx

let webApp =
    choose [
        choose [ GET; HEAD ] >=>
            choose [
                // Static cachable assets
                route  UrlPaths.``/logo.svg``   >=> svgHandler dustedCodesIcon
                route  minifiedCss.Path         >=> cssHandler

                // Health check
                route UrlPaths.``/ping``        >=> text "pong"

                // Content paths
                route    UrlPaths.``/``         >=> indexHandler
                routeCi  UrlPaths.``/about``    >=> aboutHandler
                // routeCi  UrlPaths.``/contact``  >=> aboutHandler
                routeCi  UrlPaths.``/trending`` >=> trendingHandler

                routeCi UrlPaths.``/feed/rss``  >=> rssFeedHandler
                routeCi UrlPaths.``/feed/atom`` >=> atomFeedHandler

                // Deprecated URLs kept alive in order to not break
                // existing links in the world wide web
                routeCi  UrlPaths.Deprecated.``/archive`` >=> indexHandler

                routeCif UrlPaths.``/tagged/%s`` taggedHandler
                routeCif UrlPaths.``/%s`` blogPostHandler
            ]
        notFoundHandler ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse
    >=> setStatusCode 500
    >=> (match Config.isProduction with
        | false -> Some ex.Message
        | true  -> None
        |> internalErrorView
        |> htmlView)

// ---------------------------------
// Config middleware and dependencies
// ---------------------------------

let configureServices (services : IServiceCollection) =
    services.AddResponseCaching()
            .AddMemoryCache()
            .AddGiraffe()
            |> ignore

let configureApp (app : IApplicationBuilder) =
    let forwardedHeadersOptions =
        new ForwardedHeadersOptions(
            ForwardedHeaders = ForwardedHeaders.XForwardedFor,
            ForwardLimit     = new Nullable<int>(1))

    let validateApiSecret (ctx : HttpContext) =
        match ctx.TryGetRequestHeader "X-API-SECRET" with
        | Some v -> Config.apiSecret.Equals v
        | None   -> false

    app.UseGiraffeErrorHandler(errorHandler)
       .UseForwardedHeaders(forwardedHeadersOptions)
       .UseFirewall(
            FirewallRulesEngine
                .DenyAllAccess()
                .ExceptFromCloudflare()
                .ExceptFromIPAddresses(Config.vipList)
                .ExceptWhen(fun ctx -> validateApiSecret ctx)
                .ExceptFromLocalhost())
       .UseResponseCaching()
       .UseStaticFiles()
       .UseGiraffe webApp