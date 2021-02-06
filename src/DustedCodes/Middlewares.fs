namespace DustedCodes

[<AutoOpen>]
module Middlewares =
    open System
    open System.Net
    open System.Diagnostics
    open System.Threading.Tasks
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Http.Extensions
    open Giraffe
    open FSharp.Control.Tasks

    type IApplicationBuilder with

        member this.UseErrorHandler() =
            this.Use(
                fun ctx next ->
                    unitTask {
                        try do! next.Invoke()
                        with ex ->
                            let log = ctx.GetLogFunc()
                            log Level.Critical
                                (sprintf "Unhandled exception: %s\n\n%s" ex.Message ex.StackTrace)

                            let settings = ctx.GetService<Config.Settings>()
                            ctx.Response.Clear()
                            ctx.SetStatusCode 500
                            let! _ =
                                match settings.General.IsProd with
                                | true  -> None
                                | false -> Some ex.Message
                                |> Views.internalError settings
                                |> ctx.WriteHtmlViewAsync
                            ()
                    })

        member this.UseRequestLogging(enabled, traceIdHeader) =
            match enabled with
            | false -> this
            | true  ->
                this.Use(
                    fun ctx next ->
                        unitTask {
                            let timer    = Stopwatch.StartNew()

                            let traceId =
                                ctx.TryGetRequestHeader traceIdHeader
                                |> Option.defaultValue (Guid.NewGuid().ToString())

                            let settings = ctx.GetService<Config.Settings>()

                            let logFormatter =
                                match settings.General.IsProd with
                                | true  -> Log.stackdriverFormat settings.General.AppName settings.General.AppVersion
                                | false -> Log.consoleFormat

                            let logFunc =
                                Log.write
                                    logFormatter
                                    [
                                        "RequestProtocol", ctx.Request.Protocol
                                        "RequestMethod", ctx.Request.Method
                                        "RequestURL", ctx.GetRequestUrl()
                                        "RequestPath", ctx.Request.Path.ToString()
                                        "ClientIP", ctx.Connection.RemoteIpAddress.MapToIPv4().ToString()
                                        "UserAgent", defaultArg (ctx.TryGetRequestHeader "User-Agent") ""
                                    ]
                                    (Log.parseLevel settings.General.LogLevel)
                                    traceId

                            ctx.SetLogFunc logFunc

                            let headers =
                                ctx.Request.Headers.Keys
                                |> Seq.fold(
                                    fun state key ->
                                        sprintf "%s\n    %s: %s"
                                            state
                                            key
                                            (ctx.Request.Headers.[key].ToString())) ""

                            logFunc
                                Level.Debug
                                (sprintf "%s %s %s %s"
                                    ctx.Request.Protocol
                                    ctx.Request.Method
                                    (ctx.GetRequestUrl())
                                    headers)

                            do! next.Invoke()

                            timer.Stop()

                            logFunc
                                Level.Debug
                                (sprintf "Request finished in: %s" (timer.Elapsed.ToMs()))
                        })

        member this.UseTrailingSlashRedirection(httpsPort) =
            this.Use(
                fun ctx next ->
                    let req = ctx.Request
                    let hasTrailingSlash =
                        req.Path.HasValue
                        && req.Path.Value.EndsWith "/"
                        && req.Path.Value.Length > 1

                    match hasTrailingSlash with
                    | false -> next.Invoke()
                    | true  ->
                        let path = PathString(req.Path.Value.TrimEnd '/')
                        let host = req.Host.Host
                        let hostStr =
                            match req.IsHttps, httpsPort with
                            | true, 443  -> HostString(host)
                            | true, port -> HostString(host, port)
                            | false, _   ->
                                match Nullable.toOption req.Host.Port with
                                | Some 80 -> HostString(host)
                                | Some p  -> HostString(host, p)
                                | None    -> HostString(host)
                        let req = ctx.Request
                        let url =
                            UriHelper.Encode(
                                Uri(
                                    UriHelper.BuildAbsolute(
                                        req.Scheme,
                                        hostStr,
                                        req.PathBase,
                                        path,
                                        req.QueryString),
                                    UriKind.Absolute))
                        let log = ctx.GetLogFunc()
                        log Level.Debug (sprintf "Redirecting trailing slash URL to %s" url)
                        ctx.Response.Redirect(url, true)
                        Task.CompletedTask)

        member this.UseRealIPAddress(headerName : string, proxyCount : int) =
            match proxyCount > 0 with
            | false -> this
            | true  ->
                this.Use(
                    fun ctx next ->
                        match ctx.TryGetRequestHeader headerName with
                        | Some h ->
                            let clientIP =
                                h.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                |> Array.map (fun s -> s.Trim())
                                |> Array.rev
                                |> Array.take (proxyCount + 1)
                                |> Array.rev
                                |> Array.head

                            match IPAddress.TryParse clientIP with
                            | false, _     -> ()
                            | true, ipAddr ->
                                ctx.Connection.RemoteIpAddress <- ipAddr
                                let log = ctx.GetLogFunc()
                                log Level.Debug
                                    (sprintf "Set the Client IP Address to %s from the %s header."
                                         (ipAddr.ToString())
                                         headerName)
                        | None   -> ()
                        next.Invoke())

        member this.UseHttpsRedirection
            (enabled  : bool,
            httpsHost : string,
            httpsPort : int,
            permanent : bool) =
            match enabled with
            | false -> this
            | true  ->
                this.Use(
                    fun ctx next ->
                        let expectedHost =
                            httpsHost.Split(':', 2, StringSplitOptions.RemoveEmptyEntries).[0]
                        // Only HTTPS redirect for the chosen domain:
                        let host = ctx.Request.Host.Host
                        let mustUseHttps =
                            host = expectedHost
                            || host.EndsWith ("." + expectedHost)
                        match mustUseHttps && not ctx.Request.IsHttps with
                        | false -> next.Invoke()
                        | true  ->
                            let hostStr =
                                match httpsPort with
                                | 443  -> HostString(host)
                                | port -> HostString(host, port)
                            let req = ctx.Request
                            let url =
                                UriHelper.BuildAbsolute(
                                    "https",
                                    hostStr,
                                    req.PathBase,
                                    req.Path,
                                    req.QueryString)
                            let log = ctx.GetLogFunc()
                            log Level.Debug (sprintf "Redirecting to HTTPS: %s" url)
                            ctx.Response.Redirect(url, permanent)
                            Task.CompletedTask)