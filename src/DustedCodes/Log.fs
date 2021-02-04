namespace DustedCodes

[<RequireQualifiedAccess>]
module Log =
    open System

    let private emit lvl msg =
        printfn "[%s] [%s]: %s"
            (DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:dd:ss.fff")) lvl msg

    let debugF fmt msg      = emit "DBG" (sprintf fmt msg)
    let debug msg           = emit "DBG" msg

    let infoF fmt msg       = emit "INF" (sprintf fmt msg)
    let info msg            = emit "INF" msg

    let noticeF fmt msg     = emit "NTC" (sprintf fmt msg)
    let notice msg          = emit "NTC" msg

    let warningF fmt msg    = emit "WRN" (sprintf fmt msg)
    let warning msg         = emit "WRN" msg

    let errorF fmt msg      = emit "ERR" (sprintf fmt msg)
    let error msg           = emit "ERR" msg

    let criticalF fmt msg   = emit "CRT" (sprintf fmt msg)
    let critical msg        = emit "CRT" msg

    let alertF fmt msg      = emit "ALR" (sprintf fmt msg)
    let alert msg           = emit "ALR" msg

    let emergencyF fmt msg  = emit "EMG" (sprintf fmt msg)
    let emergency msg       = emit "EMG" msg