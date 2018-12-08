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

    let isNullOrEmpty str = String.IsNullOrEmpty str

    let toOption str =
        match isNullOrEmpty str with
        | true  -> None
        | false -> Some str

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
module DevSecrets =
    open System
    open System.IO
    open System.Collections.Generic
    open Newtonsoft.Json

    let private userFolder  = Environment.GetEnvironmentVariable "HOME"
    let private secretsFile = sprintf "%s/.secrets/dustedcodes.sec.json" userFolder

    let private secrets =
        secretsFile
        |> File.Exists
        |> function
            | false -> new Dictionary<string, string>()
            | true  ->
                secretsFile
                |> File.ReadAllText
                |> JsonConvert.DeserializeObject<Dictionary<string, string>>

    let get key =
        match secrets.TryGetValue key with
        | true , value -> value
        | false, _     -> String.Empty

[<RequireQualifiedAccess>]
module Config =
    open System
    open System.IO

    let private envVar key = Environment.GetEnvironmentVariable key

    let private getSecret key =
        envVar key
        |> Str.toOption
        |> defaultArg
        <| DevSecrets.get key

    let private getOrDefault key defaultValue =
        envVar key
        |> Str.toOption
        |> defaultArg
        <| defaultValue

    let private ASPNETCORE_ENVIRONMENT      = "ASPNETCORE_ENVIRONMENT"
    let private BASE_URL                    = "BASE_URL"
    let private GOOGLE_RECAPTCHA_SITE_KEY   = "GOOGLE_RECAPTCHA_SITE_KEY"
    let private GOOGLE_RECAPTCHA_SECRET_KEY = "GOOGLE_RECAPTCHA_SECRET_KEY"
    let private GOOGLE_APIS_JSON_KEY        = "GOOGLE_APIS_JSON_KEY"
    let private GOOGLE_ANALYTICS_VIEW_ID    = "GOOGLE_ANALYTICS_VIEW_ID"
    let private LOG_LEVEL_CONSOLE           = "LOG_LEVEL_CONSOLE"
    let private LOG_LEVEL_ELASTIC           = "LOG_LEVEL_ELASTIC"
    let private VIP_LIST                    = "VIP_LIST"
    let private DISQUS_SHORTNAME            = "DISQUS_SHORTNAME"
    let private API_SECRET                  = "API_SECRET"
    let private ELASTIC_URL                 = "ELASTIC_URL"
    let private ELASTIC_USER                = "ELASTIC_USER"
    let private ELASTIC_PASSWORD            = "ELASTIC_PASSWORD"

    let contentRoot         = Directory.GetCurrentDirectory()
    let webRoot             = Path.Combine(contentRoot, "WebRoot")
    let blogPostsFolder     = Path.Combine(contentRoot, "BlogPosts")
    let staticContentFolder = Path.Combine(contentRoot, "Content")

    let blogTitle        = "Dusted Codes"
    let blogDescription  = "Programming adventures"
    let blogLanguage     = "en-GB"
    let blogAuthor       = "Dustin Moris Gorski"

    let environmentName = getOrDefault ASPNETCORE_ENVIRONMENT "Development"
    let isProduction    = environmentName |> Str.equalsCi "Production"
    let logLevelConsole = getOrDefault LOG_LEVEL_CONSOLE "error"
    let logLevelElastic = getOrDefault LOG_LEVEL_ELASTIC "warning"

    let baseUrl =
        let prodUrl  = "https://dusted.codes"
        let localUrl = "http://localhost:5000"
        getOrDefault BASE_URL (if isProduction then prodUrl else localUrl)

    let vipList =
        envVar VIP_LIST
        |> Str.toOption
        |> function
            | None      -> [||]
            | Some vips ->
                vips.Split([| ','; ' ' |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.map System.Net.IPAddress.Parse

    let apiSecret                = getSecret API_SECRET
    let googleRecaptchaSiteKey   = getSecret GOOGLE_RECAPTCHA_SITE_KEY
    let googleRecaptchaSecretKey = getSecret GOOGLE_RECAPTCHA_SECRET_KEY
    let googleApisJsonKey        = getSecret GOOGLE_APIS_JSON_KEY
    let googleAnalyticsViewId    = getSecret GOOGLE_ANALYTICS_VIEW_ID
    let disqusShortName          = getSecret DISQUS_SHORTNAME
    let elasticUrl               = getSecret ELASTIC_URL
    let elasticUser              = getSecret ELASTIC_USER
    let elasticPassword          = getSecret ELASTIC_PASSWORD

// ---------------------------------
// Urls
// ---------------------------------

[<RequireQualifiedAccess>]
module UrlPaths =
    let ``/``          = "/"
    let ``/ping``      = "/ping"
    let ``/about``     = "/about"
    let ``/hire``      = "/hire"
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
    let ``/hire``      = create UrlPaths.``/hire``
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
    open System.IO

    let content =
        Path.Combine(Config.staticContentFolder, "About.md")
        |> File.ReadAllText
        |> Markdig.Markdown.ToHtml

// ---------------------------------
// Hire
// ---------------------------------

[<RequireQualifiedAccess>]
module Hire =
    open System.IO

    let content =
        Path.Combine(Config.staticContentFolder, "Hire.md")
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

    let getMostViewedPagesAsync (jsonKey : string) (viewId : string) (maxCount : int) =
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

    let private getAllBlogPostsFromDisk (blogPostsPath : string) =
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

    let all = getAllBlogPostsFromDisk Config.blogPostsFolder

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

/// -------------------------------------
/// Http
/// -------------------------------------

[<RequireQualifiedAccess>]
module Http =
    open System.Collections.Generic
    open System.Net.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive

    let postAsync (url : string) (data : IDictionary<string, string>) =
        task {
            use client    = new HttpClient()
            let content   = new FormUrlEncodedContent(data)
            let! result   = client.PostAsync(url, content)
            let! response = result.Content.ReadAsStringAsync()
            return (result.StatusCode, response)
        }

/// -------------------------------------
/// Google reCAPTCHA
/// -------------------------------------

[<RequireQualifiedAccess>]
module Captcha =
    open System
    open System.Net
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Newtonsoft.Json

    type CaptchaValidationResult =
        | ServerError of string
        | UserError   of string
        | Success

    type CaptchaResult =
        {
            [<JsonProperty("success")>]
            IsValid        : bool

            [<JsonProperty("challenge_ts")>]
            ChallengedTime : DateTime

            [<JsonProperty("hostname")>]
            Hostname       : string

            [<JsonProperty("error-codes")>]
            ErrorCodes     : string array
        }

    let private parseError (errorCode : string) =
        match errorCode with
        | "missing-input-secret"   -> ServerError "The secret parameter is missing."
        | "invalid-input-secret"   -> ServerError "The secret parameter is invalid or malformed."
        | "missing-input-response" -> UserError "Please verify that you're not a robot."
        | "invalid-input-response" -> UserError "Verification failed. Please try again."
        | _                        -> ServerError (sprintf "Unkown error code: %s" errorCode)

    let validateAsync (secret : string) (captchaResponse : string) =
        task {
            let url = "https://www.google.com/recaptcha/api/siteverify"

            let data = dict [ "secret",   secret
                              "response", captchaResponse ]

            let! statusCode, body = Http.postAsync url data

            return
                if not (statusCode.Equals HttpStatusCode.OK)
                then ServerError body
                else
                    let result = JsonConvert.DeserializeObject<CaptchaResult> body
                    match result.IsValid with
                    | true  -> Success
                    | false -> parseError (result.ErrorCodes.[0])
        }


// ---------------------------------
// Contact page
// ---------------------------------

[<CLIMutable>]
type ContactMessage =
    {
        Name    : string
        Email   : string
        Phone   : string
        Subject : string
        Message : string
    }
    static member Empty =
        {
            Name    = ""
            Email   = ""
            Phone   = ""
            Subject = ""
            Message = ""
        }
    member __.ValidationResult =
        if      Str.isNullOrEmpty __.Name    then Error "Name cannot be empty."
        else if Str.isNullOrEmpty __.Email   then Error "Email address cannot be empty."
        else if Str.isNullOrEmpty __.Subject then Error "Subject cannot be empty."
        else if Str.isNullOrEmpty __.Message then Error "Message cannot be empty."
        else Ok ()

[<RequireQualifiedAccess>]
module DataService =
    open System
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Google.Cloud.Datastore.V1

    let private projectId = "dustins-private-project"
    let private toStringValue    (str : string)   = Value(StringValue = str)
    let private toBoolValue      (bln : bool)     = Value(BooleanValue = bln)
    let private toTimestampValue (dt  : DateTime) =
        let ts = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime dt
        Value(TimestampValue = ts)

    let saveContactMessageAsync (msg : ContactMessage) =
        task {
            try
                let kind        = "ContactMessages"
                let datastore   = DatastoreDb.Create projectId
                let keyFactory  = datastore.CreateKeyFactory(kind)
                let entity      = Entity()

                entity.Key         <- keyFactory.CreateIncompleteKey()
                entity.["Name"]    <- toStringValue    <| msg.Name
                entity.["Email"]   <- toStringValue    <| msg.Email
                entity.["Phone"]   <- toStringValue    <| msg.Phone
                entity.["Subject"] <- toStringValue    <| msg.Subject
                entity.["Message"] <- toStringValue    <| msg.Message
                entity.["Date"]    <- toTimestampValue <| DateTime.UtcNow

                let! _ = datastore.InsertAsync [ entity ]

                return Ok "Thank you, your message has been successfully sent!"
            with
            | ex -> return Error ex.Message
        }