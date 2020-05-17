namespace DustedCodes

// ---------------------------------
// Config
// ---------------------------------

[<RequireQualifiedAccess>]
module Config =
    open System
    open System.ComponentModel
    open System.Globalization

    let private strOption str =
        match String.IsNullOrEmpty str with
        | true  -> None
        | false -> Some str

    let private strSplitArray (str : string) =
        str.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

    let private tryConvertFromString<'T when 'T : struct> (cultureInfo : CultureInfo option) (value : string) =
        let culture = defaultArg cultureInfo CultureInfo.CurrentCulture
        let converter = TypeDescriptor.GetConverter (typeof<'T>)
        try Some (converter.ConvertFromString(null, culture, value) :?> 'T)
        with _ -> None

    let environmentVar key =
        Environment.GetEnvironmentVariable key
        |> strOption

    let environmentVarOrDefault key defaultValue =
        environmentVar key
        |> Option.defaultValue defaultValue

    let typedEnvironmentVar<'T when 'T : struct> culture key =
        Environment.GetEnvironmentVariable key
        |> strOption
        |> Option.bind (tryConvertFromString<'T> culture)

    let typedEnvironmentVarOrDefault<'T when 'T : struct> culture key defaultValue =
        typedEnvironmentVar<'T> culture key
        |> Option.defaultValue defaultValue

    let environmentVarList key =
        environmentVar key
        |> function
            | None   -> [||]
            | Some v -> strSplitArray v

    module CurrentCulture =
        let typedEnvironmentVar<'T when 'T : struct> key =
            Environment.GetEnvironmentVariable key
            |> strOption
            |> Option.bind (tryConvertFromString<'T> (Some CultureInfo.CurrentCulture))

        let typedEnvironmentVarOrDefault<'T when 'T : struct> (key : string) defaultValue =
            typedEnvironmentVar<'T> key
            |> Option.defaultValue defaultValue

    module InvariantCulture =
        let typedEnvironmentVar<'T when 'T : struct> key =
            Environment.GetEnvironmentVariable key
            |> strOption
            |> Option.bind (tryConvertFromString<'T> (Some CultureInfo.InvariantCulture))

        let typedEnvironmentVarOrDefault<'T when 'T : struct> (key : string) defaultValue =
            typedEnvironmentVar<'T> key
            |> Option.defaultValue defaultValue

// ---------------------------------
// Dev Config
// ---------------------------------

[<RequireQualifiedAccess>]
module DevConfig =
    open System.IO
    open System.Text.Json
    open System.Collections.Generic

    let load jsonFile =
        jsonFile
        |> File.Exists
        |> function
            | false -> Dictionary<string, string>()
            | true  ->
                jsonFile
                |> File.ReadAllText
                |> JsonSerializer.Deserialize<Dictionary<string, string>>

// ---------------------------------
// Network
// ---------------------------------

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

[<AutoOpen>]
module NetworkExtensions =
    open System
    open System.Net
    open System.Collections.Generic
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.HttpOverrides
    open Microsoft.Extensions.DependencyInjection

    type IPNetwork with
        member this.ToPrettyString() =
            sprintf "%s/%s"
                (this.Prefix.ToString())
                (this.PrefixLength.ToString())

    type IPAddress with
        member this.ToPrettyString() =
            this.MapToIPv4().ToString()

    type IEnumerable<'T> with
        member this.ToPrettyString() =
            this
            |> Seq.map (fun t ->
                match box t with
                | :? IPAddress as ip -> ip.ToPrettyString()
                | :? IPNetwork as nw -> nw.ToPrettyString()
                | _ -> t.ToString())
            |> String.concat ", "

    type IServiceCollection with
        member this.AddProxies (proxyCount    : int,
                                proxyNetworks : IPNetwork[],
                                proxies       : IPAddress[]) =

            this.Configure<ForwardedHeadersOptions>(
                fun (cfg : ForwardedHeadersOptions) ->
                    proxyNetworks
                    |> Array.iter cfg.KnownNetworks.Add
                    proxies
                    |> Array.iter cfg.KnownProxies.Add
                    cfg.RequireHeaderSymmetry <- false
                    cfg.ForwardLimit          <- Nullable<int> proxyCount
                    cfg.ForwardedHeaders      <- ForwardedHeaders.All)

    type IApplicationBuilder with
        member this.UseHttpsRedirection (domainName : string) =
            this.Use(
                fun ctx next ->
                    let host = ctx.Request.Host.Host
                    // Only HTTPS redirect for the chosen domain:
                    let mustUseHttps =
                        host = domainName
                        || host.EndsWith ("." + domainName)
                    // Otherwise prevent the HTTP redirection middleware
                    // to redirect by force setting the scheme to https:
                    if not mustUseHttps then
                        ctx.Request.Scheme <- "https"
                    next.Invoke())
                .UseHttpsRedirection()

// ---------------------------------
// Logging
// ---------------------------------

[<RequireQualifiedAccess>]
module Logging =
    open System.Text
    open System.Collections.Generic
    open Logfella

    let outputEnvironmentSummary (summary : IDictionary<string, IDictionary<string, string>>) =
        let categories = summary.Keys |> Seq.toList
        let keyLength =
            categories
            |> List.fold(
                fun (len : int) (category : string) ->
                    summary.[category].Keys
                    |> Seq.toList
                    |> List.map(fun k -> k.Length)
                    |> List.sortByDescending (fun l -> l)
                    |> List.head
                    |> max len
            ) 0
        let output =
            (categories
            |> List.fold(
                fun (sb : StringBuilder) (category : string) ->
                    summary.[category]
                    |> Seq.fold(
                        fun (sb : StringBuilder) (kvp) ->
                            let key = kvp.Key.PadLeft(keyLength, ' ')
                            let value = kvp.Value
                            sprintf "%s : %s" key value
                            |> sb.AppendLine
                    ) (sb.AppendLine("")
                         .AppendLine((sprintf "%s :" (category.ToUpper())).PadLeft(keyLength + 2, ' '))
                         .AppendLine("-----".PadRight(keyLength + 2, '-')))
            ) (StringBuilder()
                .AppendLine("")
                .AppendLine("")
                .AppendLine("..:: Environment Summary ::..")))
                .ToString()
        Log.Notice(
            output,
            ("categoryName", "startupInfo" :> obj))

[<AutoOpen>]
module LoggingExtensions =
    open System
    open Microsoft.AspNetCore.Hosting

    let private secretMask = "******"

    type String with
        member this.ToSecret() =
            match this with
            | str when String.IsNullOrEmpty str -> ""
            | str when str.Length <= 10 -> secretMask
            | str -> str.Substring(0, str.Length / 2) + secretMask

    type Option<'T> with
        member this.ToSecret() =
            match this with
            | None     -> ""
            | Some obj -> obj.ToString().ToSecret()

    type IWebHostBuilder with
        member this.ConfigureSentry (sentryDsn : string option,
                                     environmentName : string,
                                     appVersion : string) =

            match sentryDsn with
            | None -> this
            | Some dsn ->
                this.UseSentry(
                    fun sentry ->
                        sentry.Debug            <- false
                        sentry.Environment      <- environmentName
                        sentry.Release          <- appVersion
                        sentry.AttachStacktrace <- true
                        sentry.Dsn              <- dsn)

// ---------------------------------
// Str
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

// ---------------------------------
// Hashing
// ---------------------------------

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