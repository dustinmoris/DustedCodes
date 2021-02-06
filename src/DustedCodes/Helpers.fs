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
    open System.Collections.Generic
    open System.Net.Http
    open FSharp.Control.Tasks.NonAffine

    let postAsync (url : string) (data : IDictionary<string, string>) =
        task {
            use client    = new HttpClient()
            let content   = new FormUrlEncodedContent(data)
            let! result   = client.PostAsync(url, content)
            let! response = result.Content.ReadAsStringAsync()
            return (result.StatusCode, response)
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