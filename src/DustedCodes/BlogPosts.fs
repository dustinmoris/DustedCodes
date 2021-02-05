namespace DustedCodes

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

        member this.UrlEncodedTitle     = this.Title     |> WebUtility.UrlEncode
        member this.Excerpt             = this.Content.Substring(0, 100) + "..."

        member this.Permalink baseUrl
            = Url.``/%s`` baseUrl this.Id
        member this.UrlEncodedPermalink baseUrl =
            this.Permalink baseUrl |> WebUtility.UrlEncode

    type AttributeParser = string -> Article -> Result<Article, string>

    let private hashBlogPost (blogPost : Article) =
        let hash =
            (StringBuilder())
                .Append(blogPost.Title)
                .Append(blogPost.Content)
                .Append(blogPost.PublishDate.ToString())
                .ToString()
                .ToSHA1()
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
                    (fun (sb : StringBuilder) -> sb.AppendLine)
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
            Error (
                sprintf
                   "Could not parse file '%s', because it was in an unsupported format. Error message: %s."
                   blogPostPath
                   ex.Message)

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

    let load path  = getAllBlogPostsFromDisk path

    let tryFindSinglePost (blogPostsRoot : string) (id : string) =
        blogPostsRoot
        |> Directory.GetFiles
        |> Seq.find(fun f -> f.EndsWith(sprintf "%s.md" id))
        |> parseBlogPost

    let mutable all : Article list = []