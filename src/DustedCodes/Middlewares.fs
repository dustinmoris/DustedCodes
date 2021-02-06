namespace DustedCodes

[<AutoOpen>]
module Middlewares =
    open System
    open System.Diagnostics
    open System.Threading.Tasks
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Http
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

        member this.UseRequestLogging(enabled) =
            match enabled with
            | false -> this
            | true  ->
                this.Use(
                    fun ctx next ->
                        unitTask {
                            let timer    = Stopwatch.StartNew()
                            let traceId  = Guid.NewGuid().ToString()
                            let settings = ctx.GetService<Config.Settings>()

                            let logFormatter =
                                match settings.General.IsProd with
                                | true  -> Log.stackdriverFormat settings.General.AppName settings.General.AppVersion
                                | false -> Log.consoleFormat

                            let logFunc =
                                Log.write
                                    logFormatter
                                    [
                                        "Protocol", ctx.Request.Protocol
                                        "Method", ctx.Request.Method
                                        "URL", ctx.GetRequestUrl()
                                        "ClientIP", ctx.Connection.RemoteIpAddress.MapToIPv4().ToString()
                                    ]
                                    (Log.parseLevel settings.General.LogLevel)
                                    traceId

                            ctx.SetLogFunc logFunc

                            logFunc
                                Level.Debug
                                (sprintf "%s %s %s"
                                    ctx.Request.Protocol
                                    ctx.Request.Method
                                    (ctx.GetRequestUrl()))

                            do! next.Invoke()

                            timer.Stop()

                            logFunc
                                Level.Debug
                                (sprintf "Request finished in: %s" (timer.Elapsed.ToMs()))
                        })

        member this.UseTrailingSlashRedirection(httpsPort) =
            this.Use(
                fun ctx next ->
                    let hasTrailingSlash =
                        ctx.Request.Path.HasValue
                        && ctx.Request.Path.Value.EndsWith "/"
                        && ctx.Request.Path.Value.Length > 1
                    match hasTrailingSlash with
                    | true  ->
                        ctx.Request.Path <- PathString(ctx.Request.Path.Value.TrimEnd '/')
                        if ctx.Request.Scheme.EqualsCi "https" then
                            ctx.Request.Host <- HostString(ctx.Request.Host.Host, httpsPort)
                        let url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl ctx.Request
                        ctx.Response.Redirect(url, true)
                        Task.CompletedTask
                    | false -> next.Invoke())

        member this.UseHttpsRedirection (enabled, domainName) =
            match enabled with
            | false -> this
            | true  ->
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
                            ctx.Request.Scheme  <- "https"
                            ctx.Request.IsHttps <- true
                        next.Invoke())
                    .UseHttpsRedirection()