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

        member this.UseRequestLogging(enabled) =
            match enabled with
            | false -> this
            | true  ->
                this.Use(
                    fun ctx next ->
                        unitTask {
                            let timer = Stopwatch.StartNew()

                            let traceId = Guid.NewGuid().ToString()
                            sprintf "%s %s %s -- [ %s ]"
                                ctx.Request.Protocol
                                ctx.Request.Method
                                (ctx.GetRequestUrl())
                                traceId
                            |> Log.debug

                            ctx.SetTraceId traceId

                            do! next.Invoke()

                            timer.Stop()

                            sprintf "Request finished in: %s -- [ %s ]"
                                (timer.Elapsed.ToMs())
                                traceId
                            |> Log.debug
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