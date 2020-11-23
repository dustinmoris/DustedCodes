namespace DustedCodes

// ---------------------------------
// Urls
// ---------------------------------

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
    let create (route : string) =
        route.TrimStart [| '/' |]
        |> sprintf "%s/%s" Env.baseUrl

    let storage (resource : string) =
        sprintf "%s/%s" Env.storageBaseUrl resource

    let ``/``              = create UrlPaths.``/``
    let ``/about``         = create UrlPaths.``/about``
    let ``/hire``          = create UrlPaths.``/hire``
    let ``/hire#contact``  = create UrlPaths.``/hire#contact``
    let ``/trending``      = create UrlPaths.``/trending``
    let ``/feed/rss``      = create UrlPaths.``/feed/rss``
    let ``/feed/atom``     = create UrlPaths.``/feed/atom``
    let ``/logo.svg``      = create UrlPaths.``/logo.svg``

    let ``/tagged/%s`` (tag : string) = create (sprintf "/tagged/%s" tag)
    let ``/%s``        (id  : string) = create (sprintf "/%s" id)

// ---------------------------------
// About
// ---------------------------------

[<RequireQualifiedAccess>]
module MarkDog =
    open Markdig
    open Markdig.Extensions.AutoIdentifiers

    let private bone =
        MarkdownPipelineBuilder()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .Build()

    let toHtml (value : string) =
        Markdown.ToHtml(value, bone)

// ---------------------------------
// About
// ---------------------------------

[<RequireQualifiedAccess>]
module About =
    open System.IO

    let content =
        Path.Combine(Env.contentDir, "About.md")
        |> File.ReadAllText
        |> MarkDog.toHtml

// ---------------------------------
// Hire
// ---------------------------------

[<RequireQualifiedAccess>]
module Hire =
    open System.IO

    let content =
        Path.Combine(Env.contentDir, "Hire.md")
        |> File.ReadAllText
        |> MarkDog.toHtml

// ---------------------------------
// Google Analytics
// ---------------------------------

[<RequireQualifiedAccess>]
module GoogleAnalytics =
    open System
    open Google.Apis.Auth.OAuth2
    open Google.Apis.AnalyticsReporting.v4
    open Google.Apis.Services
    open Google.Apis.AnalyticsReporting.v4.Data
    open FSharp.Control.Tasks.V2.ContextInsensitive

    type PageStatistic =
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

[<RequireQualifiedAccess>]
module BlogPosts =
    open System
    open System.IO
    open System.Text
    open System.Net
    open System.Globalization

    type ContentType =
        | Html
        | Markdown

    type Article =
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

    type AttributeParser = string -> Article -> Result<Article, string>

    let private hashBlogPost (blogPost : Article) =
        let hash =
            (StringBuilder())
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

    let private parseAttribute (line : string) (blogPost : Article) =
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

    let rec private continueUntil (str : string) (blogPost : Article) (parse : AttributeParser) (lines : string array) =
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

    let rec private parseTitle (blogPost : Article, lines : string array) =
        match lines.Length = 0 with
        | true  -> Error "Unexpected end of lines. Could not finish parsing title."
        | false ->
            match lines.[0] with
            | null | "" -> parseTitle (blogPost, lines.[1..])
            | line when line.StartsWith "# " -> Ok ({ blogPost with Title = line.[2..] }, lines.[1..])
            | _ -> Error "Unexpected content. Could not parse title."

    let private readContentToEnd (blogPost : Article, lines : string array) =
        match lines.Length = 0 with
        | true  -> Ok blogPost
        | false ->
            let content =
                lines
                |> Array.fold
                    (fun (sb : StringBuilder) line -> sb.AppendLine line)
                    (StringBuilder())
                |> (fun sb -> sb.ToString())
            let htmlContent =
                match blogPost.ContentType with
                | Html     -> content
                | Markdown -> MarkDog.toHtml content
            Ok { blogPost with Content = content; HtmlContent = htmlContent }

    let private formatError (blogPostPath : string) (result : Result<Article, string>) =
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

    let all = getAllBlogPostsFromDisk Env.blogPostsDir

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
    open Giraffe.ViewEngine

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
        static member FromContent (name: string) (content : string) =
            let hash = Hash.sha1 content
            {
                Content = content
                Hash    = hash
                Path    = sprintf "/%s.%s.css" name hash
            }

    let private getErrorMsg (errors : seq<UglifyError>) =
        let msg =
            errors
            |> Seq.fold (fun (sb : StringBuilder) t ->
                sprintf "Error: %s, File: %s" t.Message t.File
                |> sb.AppendLine
            ) (StringBuilder("Couldn't uglify content."))
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

    let getBundledContent (bundleName : string) (fileNames : string list) =
        let result =
            fileNames
            |> List.fold(
                fun (sb : StringBuilder) fileName ->
                    fileName
                    |> getMinifiedContent
                    |> sb.AppendLine
            ) (StringBuilder())
        result.ToString()
        |> BundledCss.FromContent bundleName

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
        | _                        -> ServerError (sprintf "Unknown error code: %s" errorCode)

    let validate (secret : string) (captchaResponse : string) =
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
// Contact Messages
// ---------------------------------

[<RequireQualifiedAccess>]
module ContactMessages =
    open System

    [<CLIMutable>]
    type Entity =
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

        member this.IsValid =
            if      String.IsNullOrEmpty this.Name    then Error "Name cannot be empty."
            else if String.IsNullOrEmpty this.Email   then Error "Email address cannot be empty."
            else if String.IsNullOrEmpty this.Subject then Error "Subject cannot be empty."
            else if String.IsNullOrEmpty this.Message then Error "Message cannot be empty."
            else Ok ()

// ---------------------------------
// Data Service
// ---------------------------------

[<RequireQualifiedAccess>]
module DataService =
    open System
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Google.Cloud.Datastore.V1
    open Logfella

    let private toStringValue (str : string) = Value(StringValue = str)

    let private toTextValue (str : string) =
        let v = Value(StringValue = str)
        v.ExcludeFromIndexes <- true
        v

    let private toTimestampValue (dt  : DateTime) =
        let ts = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime dt
        Value(TimestampValue = ts)

    let private entityFromContactMessage
        (key : Key)
        (msg : ContactMessages.Entity) =

        let entity     = Entity()
        entity.Key             <- key
        entity.["Name"]        <- msg.Name         |> toStringValue
        entity.["Email"]       <- msg.Email        |> toStringValue
        entity.["Phone"]       <- msg.Phone        |> toStringValue
        entity.["Subject"]     <- msg.Subject      |> toStringValue
        entity.["Message"]     <- msg.Message      |> toTextValue
        entity.["Date"]        <- DateTime.UtcNow  |> toTimestampValue
        entity.["Origin"]      <- Env.appName      |> toStringValue
        entity.["Environment"] <- Env.name         |> toStringValue
        entity

    let private save (datastore : DatastoreDb)  (entity : Entity) =
        task {
            try
                let! keys = datastore.InsertAsync [ entity ]
                let foldedKeys =
                    keys
                    |> Seq.fold(fun str k -> str + k.ToString() + " ") " "
                Log.Notice(
                    "A new entity has been successfully saved.",
                    ("dataKeys", foldedKeys :> obj))
                return Ok keys
            with ex ->
                Log.Alert("Failed to save entity in Google Cloud Datastore.", ex)
                return Error ex.Message
        }

    let saveContactMessage (msg : ContactMessages.Entity) =
        Log.Debug "Initialising Google Cloud DatastoreDb..."
        let datastore  = DatastoreDb.Create Env.gcpProjectId
        Log.Debug "Creating key factory for entity kind..."
        let keyFactory = datastore.CreateKeyFactory Env.gcpContactMessageKind
        Log.Debug "Generating new entity key..."
        let key        = keyFactory.CreateIncompleteKey()
        Log.Debug "Creating and saving entity..."
        let keys =
            msg
            |> entityFromContactMessage key
            |> save datastore
        Log.Debug "Contact message has been successfully saved."
        keys

// ---------------------------------
// Email Service
// ---------------------------------

module EmailService =
    open System
    open System.Collections.Generic
    open Google.Protobuf
    open Google.Cloud.PubSub.V1
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Newtonsoft.Json
    open Logfella

    [<CLIMutable>]
    type Message =
        {
            Domain       : string
            Sender       : string
            Recipients   : string list
            CC           : string list
            BCC          : string list
            Subject      : string
            TemplateName : string
            TemplateData : IDictionary<string, string>
        }

    let private sendMessage (msg : Message) =
        task {
            try
                let topicName  = TopicName(Env.gcpProjectId, Env.gcpContactMessageTopic)
                let! publisher = PublisherClient.CreateAsync topicName
                let data =
                    msg
                    |> JsonConvert.SerializeObject
                    |> ByteString.CopyFromUtf8
                let pubSubMsg = PubsubMessage(Data = data)
                pubSubMsg.Attributes.Add("encoding", "json-utf8")
                let! messageId = publisher.PublishAsync(pubSubMsg)
                do! publisher.ShutdownAsync(TimeSpan.FromSeconds 15.0)
                Log.Notice(
                     "A new contact message has been successfully sent.",
                     ("messageId", messageId :> obj))
                return Ok messageId
            with ex ->
                Log.Alert(
                    "Failed to publish to emails topic in Google Cloud PubSub.",
                    ex)
                return Error ex.Message
        }

    let sendContactMessage (msg : ContactMessages.Entity) =
        {
            Domain       = Env.mailDomain
            Sender       = Env.mailSender
            Recipients   = [ Env.contactMessagesRecipient ]
            CC           = []
            BCC          = []
            Subject      = "Dusted Codes: A new message has been posted"
            TemplateName = "contact-message"
            TemplateData =
                dict [
                    "environmentName", Env.name
                    "msgSubject",      msg.Subject
                    "msgContent",      msg.Message
                    "msgDate",         DateTimeOffset.Now.ToString("u")
                    "msgSenderName",   msg.Name
                    "msgSenderEmail",  msg.Email
                    "msgSenderPhone",  msg.Phone
                ]
        }
        |> sendMessage