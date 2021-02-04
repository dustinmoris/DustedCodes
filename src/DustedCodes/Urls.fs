namespace DustedCodes

[<RequireQualifiedAccess>]
module UrlPaths =
    let ``/``              = "/"
    let ``/ping``          = "/ping"
    let ``/version``       = "/version"
    let ``/about``         = "/about"
    let ``/hire``          = "/hire"
    let ``/hire#contact``  = "/hire#contact"
    let ``/trending``      = "/trending"
    let ``/feed/rss``      = "/feed/rss"
    let ``/feed/atom``     = "/feed/atom"
    let ``/logo.svg``      = "/logo.svg"

    let ``/tagged/%s`` : PrintfFormat<string -> obj, obj, obj, obj, string> = "/tagged/%s"
    let ``/%s``        : PrintfFormat<string -> obj, obj, obj, obj, string> = "/%s"

    module Deprecated =
        let ``/archive`` = "/archive"

    module Debug =
        let ``/error`` = "/error"

[<RequireQualifiedAccess>]
module Url =
    let create (baseUrl : string) (route : string) =
        route.TrimStart [| '/' |]
        |> sprintf "%s/%s" baseUrl

    let storage (storageBaseUrl) (resource : string) =
        sprintf "%s/%s" storageBaseUrl resource

    let ``/``             baseUrl = create baseUrl UrlPaths.``/``
    let ``/about``        baseUrl = create baseUrl UrlPaths.``/about``
    let ``/hire``         baseUrl = create baseUrl UrlPaths.``/hire``
    let ``/hire#contact`` baseUrl = create baseUrl UrlPaths.``/hire#contact``
    let ``/trending``     baseUrl = create baseUrl UrlPaths.``/trending``
    let ``/feed/rss``     baseUrl = create baseUrl UrlPaths.``/feed/rss``
    let ``/feed/atom``    baseUrl = create baseUrl UrlPaths.``/feed/atom``
    let ``/logo.svg``     baseUrl = create baseUrl UrlPaths.``/logo.svg``

    let ``/tagged/%s`` baseUrl (tag : string) = create baseUrl (sprintf "/tagged/%s" tag)
    let ``/%s``        baseUrl (id  : string) = create baseUrl (sprintf "/%s" id)