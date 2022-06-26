namespace DustedCodes

/// -------------------------------------
/// Nullable
/// -------------------------------------

module Nullable =
    open System

    let toOption (nullable : Nullable<'T>) =
        match nullable.HasValue with
        | true  -> Some nullable.Value
        | false -> None

/// -------------------------------------
/// Network
/// -------------------------------------

[<RequireQualifiedAccess>]
module Network =
    open System
    open System.Net
    open Microsoft.AspNetCore.HttpOverrides

    let tryParseIPAddress (str : string) =
        match IPAddress.TryParse str with
        | true, ipAddress -> Some ipAddress
        | false, _        -> None

    let tryParseNetworkAddress (str : string) =
        let ipAddr, cidrLen =
            match str.Split('/', StringSplitOptions.RemoveEmptyEntries) with
            | arr when arr.Length = 2 -> arr.[0], Some arr.[1]
            | arr -> arr.[0], None

        match IPAddress.TryParse ipAddr with
        | false, _        -> None
        | true, ipAddress ->
            let cidrMask =
                match cidrLen with
                | None     -> None
                | Some len ->
                    match Int32.TryParse len with
                    | true, mask -> Some mask
                    | false, _   -> None
            match cidrMask with
            | Some mask -> Some (IPNetwork(ipAddress, mask))
            | None      -> Some (IPNetwork(ipAddress, 32))

/// -------------------------------------
/// Http
/// -------------------------------------

[<RequireQualifiedAccess>]
module Http =
    open System.Text
    open System.Net.Http
    open System.Threading
    open System.Threading.Tasks
    open System.Collections.Generic
    open Newtonsoft.Json

    type PostResult   = Task<Result<string, string>>
    type PostFormFunc = IDictionary<string, string> -> CancellationToken -> PostResult
    type PostJsonFunc = obj -> CancellationToken -> PostResult

    let private postReq
        (client : HttpClient)
        (req    : HttpRequestMessage)
        (ct     : CancellationToken) : PostResult =
        task {
            try
                let! resp = client.SendAsync(req, ct)
                let! body = resp.Content.ReadAsStringAsync()
                return
                    match resp.IsSuccessStatusCode with
                    | true  -> Ok body
                    | false -> Error body
            with ex ->
                JsonConvert.SerializeObject(ex, Formatting.Indented) |> System.Console.WriteLine
                return Error ex.Message
        }

    let postForm
        (client : HttpClient)
        (form   : IDictionary<string, string>)
        (ct     : CancellationToken) =
        task {
            use data = new FormUrlEncodedContent(form)
            use req  = new HttpRequestMessage(Method = HttpMethod.Post, Content = data)
            return! postReq client req ct
        }

    let postJson
        (client : HttpClient)
        (data   : obj)
        (ct     : CancellationToken) =
        task {
            let json = JsonConvert.SerializeObject data
            use data = new StringContent(json, Encoding.UTF8, "application/json")
            use req  = new HttpRequestMessage(Method = HttpMethod.Post, Content = data)
            return! postReq client req ct
        }

// ---------------------------------
// Markdown
// ---------------------------------

[<RequireQualifiedAccess>]
module MarkDog =
    open Markdig
    open Markdig.Extensions.AutoIdentifiers

    let private bone =
        MarkdownPipelineBuilder()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .UsePipeTables()
            .Build()

    let toHtml (value : string) =
        Markdown.ToHtml(value, bone)