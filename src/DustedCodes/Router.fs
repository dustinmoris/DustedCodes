namespace DustedCodes

[<RequireQualifiedAccess>]
module Router =
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