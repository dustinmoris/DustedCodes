namespace DustedCodes

type Level =
    | Debug     = 100
    | Info      = 200
    | Notice    = 300
    | Warning   = 400
    | Error     = 500
    | Critical  = 600
    | Alert     = 700
    | Emergency = 800

[<RequireQualifiedAccess>]
module Log =
    open System
    open System.Web
    open Microsoft.AspNetCore.Http
    open Giraffe

    type Event =
        {
            Timestamp   : DateTimeOffset
            TraceId     : string
            SpanId      : string
            Level       : Level
            Message     : string
            Labels      : (string * string) list
            HttpCtx     : HttpContext option
        }

    type FormatFunc = Event -> string
    type Func    = Level -> string -> unit

    let write
        (format     : FormatFunc)
        (labels     : (string * string) list)
        (minLevel   : Level)
        (httpCtx    : HttpContext option)
        (traceId    : string)
        (spanId     : string) : Func =
        fun (level : Level) (message : string) ->
            if level >= minLevel then
                {
                    Timestamp = DateTimeOffset.UtcNow
                    TraceId   = traceId
                    SpanId    = spanId
                    Level     = level
                    Message   = message
                    Labels    = labels
                    HttpCtx   = httpCtx
                }
                |> format
                |> printfn "%s"

    let private shortLevel (level : Level) =
        match level with
        | Level.Debug     -> "DBG"
        | Level.Info      -> "INF"
        | Level.Notice    -> "NTC"
        | Level.Warning   -> "WRN"
        | Level.Error     -> "ERR"
        | Level.Critical  -> "CRT"
        | Level.Alert     -> " ! "
        | Level.Emergency -> "!!!"
        | _               -> "???"

    let private longLevel (level : Level) =
        match level with
        | Level.Debug     -> "Debug"
        | Level.Info      -> "Info"
        | Level.Notice    -> "Notice"
        | Level.Warning   -> "Warning"
        | Level.Error     -> "Error"
        | Level.Critical  -> "Critical"
        | Level.Alert     -> "Alert"
        | Level.Emergency -> "Emergency"
        | _               -> "???"

    let parseLevel (level : string) =
        match level.ToLowerInvariant() with
        | "debug"       -> Level.Debug
        | "info"        -> Level.Info
        | "information" -> Level.Info
        | "notice"      -> Level.Notice
        | "warning"     -> Level.Warning
        | "error"       -> Level.Error
        | "critical"    -> Level.Critical
        | "alert"       -> Level.Alert
        | "emergency"   -> Level.Emergency
        | _             -> Level.Warning

    let consoleFormat (e : Event) =
        sprintf "[%s] [%s] [%s]: %s"
            (DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:dd:ss.fff"))
            e.TraceId
            (shortLevel e.Level)
            e.Message

    let private encodeJson = HttpUtility.JavaScriptStringEncode

    let stackdriverFormat (serviceName : string) (serviceVersion : string) =
        fun (e : Event) ->
            let svcName     = encodeJson serviceName
            let svcVersion  = encodeJson serviceVersion
            let defaultLabels =
                sprintf "\"appName\": \"%s\", \"appVersion\": \"%s\""
                    svcName
                    svcVersion
            let labels =
                e.Labels
                |> List.fold(
                    fun state (key, value) ->
                        sprintf "%s ,\"%s\": \"%s\"" state key (encodeJson value))
                    defaultLabels
            let httpReq =
                match e.HttpCtx with
                | None -> ""
                | Some h ->
                    let reqSize =
                        match h.Request.ContentLength.HasValue with
                        | true  -> h.Request.ContentLength.Value
                        | false -> 0L
                    let userAgent =
                        defaultArg (h.TryGetRequestHeader "User-Agent") ""
                    let referer =
                        defaultArg (h.TryGetRequestHeader "Referer") ""
                    sprintf ", \"httpRequest\": { \"protocol\": \"%s\", \"requestMethod\": \"%s\", \"requestUrl\": \"%s\", \"requestSize\": \"%d\", \"userAgent\": \"%s\", \"remoteIp\": \"%s\", \"referer\": \"%s\" }"
                        h.Request.Protocol
                        h.Request.Method
                        (h.GetRequestUrl())
                        reqSize
                        userAgent
                        (h.Connection.RemoteIpAddress.ToString())
                        referer
            sprintf
                "{ \"severity\": \"%s\", \"message\": \"%s\", \"serviceContext.service\": \"%s\", \"serviceContext.version\": \"%s\", \"logging.googleapis.com/labels\": { %s }, \"logging.googleapis.com/trace\": \"%s\", \"logging.googleapis.com/spanId\": \"%s\", \"logging.googleapis.com/trace_sampled\": \"true\"%s }"
                (longLevel e.Level)
                (encodeJson e.Message)
                svcName
                svcVersion
                labels
                e.TraceId
                e.SpanId
                httpReq