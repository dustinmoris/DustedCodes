namespace DustedCodes

// ---------------------------------
// Common
// ---------------------------------

[<RequireQualifiedAccess>]
module Str =
    open System

    let private ignoreCase = StringComparison.InvariantCultureIgnoreCase
    let equals   (s1 : string) (s2 : string) = s1.Equals s2
    let equalsCi (s1 : string) (s2 : string) = s1.Equals(s2, ignoreCase)

[<RequireQualifiedAccess>]
module Hash =
    open System.Text
    open System.Security.Cryptography

    let sha1 (str : string) =
        str
        |> Encoding.UTF8.GetBytes
        |> SHA1.Create().ComputeHash
        |> Array.map (fun b -> b.ToString "x2")
        |> String.concat ""

// ---------------------------------
// Config
// ---------------------------------

[<RequireQualifiedAccess>]
module Config =
    open System
    open System.IO

    let private envVar (key : string) = Environment.GetEnvironmentVariable key

    let private ASPNETCORE_ENVIRONMENT   = envVar "ASPNETCORE_ENVIRONMENT"
    let private BASE_URL                 = envVar "BASE_URL"
    let private GOOGLE_APIS_JSON_KEY     = envVar "GOOGLE_APIS_JSON_KEY"
    let private GOOGLE_ANALYTICS_VIEW_ID = envVar "GOOGLE_ANALYTICS_VIEW_ID"
    let private LOG_LEVEL                = envVar "LOG_LEVEL"
    let private VIP_LIST                 = envVar "VIP_LIST"
    let private DISQUS_SHORTNAME         = envVar "DISQUS_SHORTNAME"

    let contentRoot         = Directory.GetCurrentDirectory()
    let webRoot             = Path.Combine(contentRoot, "WebRoot")
    let blogPostsFolder     = Path.Combine(contentRoot, "BlogPosts")
    let staticContentFolder = Path.Combine(contentRoot, "Content")

    let blogTitle        = "Dusted Codes"
    let blogDescription  = "Programming adventures"
    let blogLanguage     = "en-GB"
    let blogAuthor       = "Dustin Moris Gorski"

    let isProduction =
        ASPNETCORE_ENVIRONMENT
        |> String.IsNullOrEmpty
        |> function
            | false -> ASPNETCORE_ENVIRONMENT |> Str.equalsCi "Production"
            | true  -> false

    let logLevel =
        LOG_LEVEL
        |> String.IsNullOrEmpty
        |> function
            | false -> LOG_LEVEL
            | true  -> "error"

    let baseUrl =
        let prodUrl  = "https://dusted.codes"
        let localUrl = "http://localhost:5000"
        BASE_URL
        |> String.IsNullOrEmpty
        |> function
            | false -> BASE_URL
            | true  -> if isProduction then prodUrl else localUrl

    let vipList =
        VIP_LIST
        |> String.IsNullOrEmpty
        |> function
            | true  -> [||]
            | false ->
                VIP_LIST.Split([| ','; ' ' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map System.Net.IPAddress.Parse

    let googleApisJsonKey =
        GOOGLE_APIS_JSON_KEY
        |> String.IsNullOrEmpty
        |> function
            | false -> GOOGLE_APIS_JSON_KEY
            | true  ->
                Giraffe.Common.readFileAsStringAsync "/Users/dustinmoris/Private/google-analytics-sa-key.json"
                |> Async.AwaitTask
                |> Async.RunSynchronously

    let googleAnalyticsViewId =
        GOOGLE_ANALYTICS_VIEW_ID
        |> String.IsNullOrEmpty
        |> function
            | true  ->
                Giraffe.Common.readFileAsStringAsync "/Users/dustinmoris/Private/google-analytics-view-id.txt"
                |> Async.AwaitTask
                |> Async.RunSynchronously
            | false -> GOOGLE_ANALYTICS_VIEW_ID

    let disqusShortName =
        DISQUS_SHORTNAME
        |> Giraffe.Common.strOption
        |> defaultArg
        <| "dev-dustedcodes"

// ---------------------------------
// Urls
// ---------------------------------

[<RequireQualifiedAccess>]
module UrlPaths =
    let ``/``          = "/"
    let ``/about``     = "/about"
    let ``/contact``   = "/contact"
    let ``/trending``  = "/trending"
    let ``/feed/rss``  = "/feed/rss"
    let ``/feed/atom`` = "/feed/atom"
    let ``/logo.svg``  = "/logo.svg"

    let ``/tagged/%s`` : PrintfFormat<string -> obj, obj, obj, obj, string> = "/tagged/%s"
    let ``/%s``        : PrintfFormat<string -> obj, obj, obj, obj, string> = "/%s"

    module Deprecated =
        let ``/archive`` = "/archive"

[<RequireQualifiedAccess>]
module Url =
    let create (route : string) =
        route.TrimStart [| '/' |]
        |> sprintf "%s/%s" Config.baseUrl

    let ``/``          = create UrlPaths.``/``
    let ``/about``     = create UrlPaths.``/about``
    let ``/contact``   = create UrlPaths.``/contact``
    let ``/trending``  = create UrlPaths.``/trending``
    let ``/feed/rss``  = create UrlPaths.``/feed/rss``
    let ``/feed/atom`` = create UrlPaths.``/feed/atom``
    let ``/logo.svg``  = create UrlPaths.``/logo.svg``

    let ``/tagged/%s`` (tag : string) = create (sprintf "/tagged/%s" tag)
    let ``/%s``        (id  : string) = create (sprintf "/%s" id)

// ---------------------------------
// About
// ---------------------------------

[<RequireQualifiedAccess>]
module About =
    open System
    open System.IO

    let daysInLondon =
        let dateMovedToLondon  = DateTime(2012, 10, 16)
        let timeLivingInLondon = DateTime.UtcNow - dateMovedToLondon
        let daysPerYear        = 365
        timeLivingInLondon.Days / daysPerYear

    let content =
        Path.Combine(Config.staticContentFolder, "About.md")
        |> File.ReadAllText
        |> Markdig.Markdown.ToHtml

// ---------------------------------
// Google Analytics
// ---------------------------------

module GoogleAnalytics =
    open System
    open Google.Apis.Auth.OAuth2
    open Google.Apis.AnalyticsReporting.v4
    open Google.Apis.Services
    open Google.Apis.AnalyticsReporting.v4.Data
    open FSharp.Control.Tasks.V2.ContextInsensitive

    type PageViewStatistic =
        {
            Path      : string
            ViewCount : int64
        }

    let getMostViewedPages (jsonKey : string) (viewId : string) (maxCount : int) =
        task {
            let credential =
                GoogleCredential
                    .FromJson(jsonKey)
                    .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly)

            use service =
                new AnalyticsReportingService(
                    BaseClientService.Initializer(
                        ApplicationName       = "Dusted Codes Website",
                        HttpClientInitializer = credential))

            let reportRequest =
                ReportRequest(
                    ViewId     = viewId,
                    DateRanges = [|
                        DateRange(
                            StartDate = DateTime(2015, 02, 16).ToString("yyyy-MM-dd"),
                            EndDate   = DateTime.UtcNow.ToString("yyyy-MM-dd")) |],
                    Metrics    = [| Metric (Expression = "ga:pageviews") |],
                    Dimensions = [| Dimension(Name = "ga:pagePath") |],
                    OrderBys   = [| OrderBy(FieldName = "ga:pageviews", SortOrder = "DESCENDING") |]
                )

            let request =
                service.Reports.BatchGet(
                    GetReportsRequest(
                        ReportRequests = [| reportRequest |]))

            let! response = request.ExecuteAsync()
            let report    = response.Reports.[0]
            let maxRows   = min report.Data.Rows.Count maxCount

            return
                report.Data.Rows
                |> Seq.take maxRows
                |> Seq.map (fun r -> { Path      = r.Dimensions.[0];
                                       ViewCount = int64 r.Metrics.[0].Values.[0] })
                |> Seq.toList
        }

// ---------------------------------
// Blog Posts
// ---------------------------------

module BlogPosts =
    open System
    open System.IO
    open System.Text
    open System.Net
    open System.Globalization
    open Markdig

    type ContentType =
        | Html
        | Markdown

    type BlogPost =
        {
            Id          : string
            Title       : string
            PublishDate : DateTimeOffset
            Tags        : string list option
            ContentType : ContentType
            Content     : string
            HtmlContent : string
            HashCode    : string
        }
        member this.Permalink           = Url.``/%s`` this.Id
        member this.UrlEncodedPermalink = this.Permalink |> WebUtility.UrlEncode
        member this.UrlEncodedTitle     = this.Title     |> WebUtility.UrlEncode
        member this.Excerpt             = this.Content.Substring(0, 100) + "..."

    type AttributeParser = string -> BlogPost -> Result<BlogPost, string>

    let private hashBlogPost (blogPost : BlogPost) =
        let hash =
            (new StringBuilder())
                .Append(blogPost.Title)
                .Append(blogPost.Content)
                .Append(blogPost.PublishDate.ToString())
                .ToString()
            |> Hash.sha1
        Ok { blogPost with HashCode = hash }


    let private parseContentType (input : string) =
        match input.ToLower() with
        | "html" -> Html
        | _      -> Markdown

    let private parseTags (input : string) =
        if String.IsNullOrEmpty input then None
        else
            input.Split([| ' '; ','; ';'; '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.toList
            |> Some

    let private splitIntoKeyValue (line : string) =
        let two   = line.Split([| ':' |], 2, StringSplitOptions.None)
        match two.Length with
        | 2 ->
            let key   = two.[0].Trim().ToLower()
            let value = two.[1].Trim()
            Ok (key, value)
        | _ -> Error "Could not parse data due to badly formatted key value pair."

    let private parseAttribute (line : string) (blogPost : BlogPost) =
        line
        |> splitIntoKeyValue
        |> Result.bind (fun (key, value) ->
            match key with
            | "tags" -> Ok { blogPost with Tags = value |> parseTags }
            | "type" -> Ok { blogPost with ContentType = value |> parseContentType }
            | _      -> Ok blogPost)

    let private mustNotBeEmpty (lines : string array) =
        match lines.Length = 0 with
        | true  -> Error "Cannot parse empty content."
        | false -> Ok lines

    let private beginsWith (str : string) (lines : string array) =
        match (lines.[0]).StartsWith str with
        | true  -> Ok (lines.[1..])
        | false -> Error (sprintf "Content was expected to begin with %s" str)

    let rec private continueUntil (str : string) (blogPost : BlogPost) (parse : AttributeParser) (lines : string array) =
        match lines.Length = 0 with
        | true  -> Error "Unexpected end of lines. Could not finish parsing attributes."
        | false ->
            match lines.[0] with
            | null | "" -> continueUntil str blogPost parse lines.[1..]
            | line when line.Equals str -> Ok (blogPost, lines.[1..])
            | line ->
                match parse line blogPost with
                | Ok result -> continueUntil str result parse lines.[1..]
                | Error msg -> Error msg

    let rec private parseTitle (blogPost : BlogPost, lines : string array) =
        match lines.Length = 0 with
        | true  -> Error "Unexpected end of lines. Could not finish parsing title."
        | false ->
            match lines.[0] with
            | null | "" -> parseTitle (blogPost, lines.[1..])
            | line when line.StartsWith "# " -> Ok ({ blogPost with Title = line.[2..] }, lines.[1..])
            | _ -> Error "Unexpected content. Could not parse title."

    let private readContentToEnd (blogPost : BlogPost, lines : string array) =
        match lines.Length = 0 with
        | true  -> Ok blogPost
        | false ->
            let content =
                lines
                |> Array.fold
                    (fun (sb : StringBuilder) line -> sb.AppendLine line)
                    (new StringBuilder())
                |> (fun sb -> sb.ToString())
            let htmlContent =
                match blogPost.ContentType with
                | Html     -> content
                | Markdown -> Markdown.ToHtml content
            Ok { blogPost with Content = content; HtmlContent = htmlContent }

    let private formatError (blogPostPath : string) (result : Result<BlogPost, string>) =
        match result with
        | Ok _      -> result
        | Error msg -> Error (sprintf "Could not parse file '%s'. Error message : %s." blogPostPath msg)

    let private parseBlogPost (blogPostPath : string) =
        try
            let file  = FileInfo(blogPostPath)
            let parts = file.Name.Replace(".md", "").Split([| "-" |], 2, StringSplitOptions.None)
            let date  = DateTimeOffset.ParseExact(parts.[0], "yyyy_MM_dd", CultureInfo.InvariantCulture)
            let id    = parts.[1]
            let lines = File.ReadAllLines blogPostPath
            let blogPost =
                {
                    Id          = id
                    Title       = ""
                    PublishDate = date
                    Tags        = None
                    ContentType = Markdown
                    Content     = ""
                    HtmlContent = ""
                    HashCode    = ""
                }
            lines
            |> mustNotBeEmpty
            |> Result.bind (beginsWith "<!--")
            |> Result.bind (continueUntil "-->" blogPost parseAttribute)
            |> Result.bind parseTitle
            |> Result.bind readContentToEnd
            |> Result.bind hashBlogPost
            |> formatError blogPostPath
        with ex ->
            Error (sprintf "Could not parse file '%s', because it was in an unsupported format. Error message: %s." blogPostPath ex.Message)

    let getAllBlogPostsFromDisk (blogPostsPath : string) =
        blogPostsPath
        |> Directory.GetFiles
        |> Array.map parseBlogPost
        |> Array.fold (fun (blogPosts, errors) result ->
            match result with
            | Ok blogPost -> blogPosts @ [ blogPost ], errors
            | Error msg  -> blogPosts, errors @ [ msg ]) (List.empty, List.empty)
        |> (fun (blogPosts, errors) ->
            match errors.Length = 0 with
            | true -> blogPosts
            | false ->
                errors
                |> List.fold (fun acc line -> acc + Environment.NewLine + line) ""
                |> failwith)

// ---------------------------------
// RSS Feed
// ---------------------------------

[<RequireQualifiedAccess>]
module Feed =
    open System

    type Item =
        {
            Guid        : string
            Link        : string
            Title       : string
            Description : string
            PubDate     : string
            Author      : string
            Source      : string
            Categories  : string list option
        }
        static member Create (permalink   : string)
                             (title       : string)
                             (content     : string)
                             (publishDate : DateTimeOffset)
                             (author      : string)
                             (feedUrl     : string)
                             (categories  : string list option) =
            {
                Guid        = permalink
                Link        = permalink
                Title       = title
                Description = content
                PubDate     = publishDate.ToString("R")
                Author      = author
                Source      = feedUrl
                Categories  = categories
            }

    type Channel =
        {
            Link        : string
            Title       : string
            Description : string
            Language    : string
            Generator   : string
            Items       : Item list
        }
        static member Create (channelUrl  : string)
                             (title       : string)
                             (description : string)
                             (language    : string)
                             (generator   : string)
                             (items       : Item list) =
            {
                Link        = channelUrl
                Title       = title
                Description = description
                Language    = language
                Generator   = generator
                Items       = items
            }

[<RequireQualifiedAccess>]
module RssFeed =
    open Giraffe.GiraffeViewEngine

    let create (channel : Feed.Channel) =
        let title       = tag "title"
        let link        = tag "link"
        let description = tag "description"

        tag "rss" [ attr "version" "2.0" ] [
            tag "channel" [] [
                yield title       [] [ encodedText channel.Title ]
                yield link        [] [ encodedText channel.Link ]
                yield description [] [ encodedText channel.Description ]
                yield tag "language"  [] [ encodedText channel.Language ]
                yield tag "generator" [] [ encodedText channel.Generator ]
                yield!
                    channel.Items
                    |> List.map (fun i ->
                        tag "item" [] [
                            yield tag "guid"  [] [ encodedText i.Guid ]
                            yield link        [] [ encodedText i.Link ]
                            yield title       [] [ encodedText i.Title ]
                            yield description [] [ encodedText i.Description ]
                            yield tag "pubDate" [] [ encodedText i.PubDate ]
                            yield tag "author"  [] [ encodedText i.Author ]
                            yield tag "source"  [] [ encodedText i.Source ]
                            if i.Categories.IsNone then
                                yield!
                                    i.Categories.Value
                                    |> List.map (fun c -> tag "category" [] [ encodedText c ])
                        ]
                    )
            ]
        ]

// ---------------------------------
// Minification
// ---------------------------------

[<RequireQualifiedAccess>]
module Css =
    open System.IO
    open System.Text
    open NUglify

    type BundledCss =
        {
            Content : string
            Hash    : string
            Path    : string
        }
        static member FromContent (content : string) =
            let hash = Hash.sha1 content
            {
                Content = content
                Hash    = hash
                Path    = sprintf "/%s.css" hash
            }

    let private getErrorMsg (errors : UglifyError seq) =
        let msg =
            errors
            |> Seq.fold (fun (sb : StringBuilder) t ->
                sprintf "Error: %s, File: %s" t.Message t.File
                |> sb.AppendLine
            ) (new StringBuilder("Couldn't uglify content."))
        msg.ToString()

    let minify (css : string) =
        css
        |> Uglify.Css
        |> (fun res ->
            match res.HasErrors with
            | true  -> failwith (getErrorMsg res.Errors)
            | false -> res.Code)

    let getMinifiedContent (fileName : string) =
        fileName
        |> File.ReadAllText
        |> minify

    let getBundledContent (fileNames : string list) =
        let result =
            fileNames
            |> List.fold(
                fun (sb : StringBuilder) fileName ->
                    fileName
                    |> getMinifiedContent
                    |> sb.AppendLine
            ) (new StringBuilder())
        result.ToString()
        |> BundledCss.FromContent