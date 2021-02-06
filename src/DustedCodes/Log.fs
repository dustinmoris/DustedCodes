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

    type Event =
        {
            Timestamp   : DateTimeOffset
            TraceId     : string
            Level       : Level
            Message     : string
            Labels      : (string * string) list
        }

    type FormatFunc = Event -> string
    type Func    = Level -> string -> unit

    let write
        (format     : FormatFunc)
        (labels     : (string * string) list)
        (minLevel   : Level)
        (traceId    : string) : Func =
        fun (level : Level) (message : string) ->
            if level >= minLevel then
                {
                    Timestamp = DateTimeOffset.UtcNow
                    TraceId   = traceId
                    Level     = level
                    Message   = message
                    Labels    = labels
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

    let private encodeJson = System.Web.HttpUtility.JavaScriptStringEncode

    let stackdriverFormat (serviceName : string) (serviceVersion : string) =
        fun (e : Event) ->
            ("appName", serviceName) :: ("appVersion", serviceVersion) :: e.Labels
            |> List.fold(
                fun state (key, value) ->
                    sprintf "%s ,\"%s\": \"%s\"" state key (encodeJson value))
                (sprintf "\"traceId\": \"%s\"" e.TraceId)
            |> sprintf
                "{ \"severity\": \"%s\", \"message\": \"%s\", \"serviceContext.service\": \"%s\", \"serviceContext.version\": \"%s\", \"logging.googleapis.com/labels\": { %s } }"
                (longLevel e.Level)
                (encodeJson e.Message)
                serviceName
                serviceVersion