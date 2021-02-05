namespace DustedCodes

[<RequireQualifiedAccess>]
module Captcha =
    open System
    open System.Net
    open System.Diagnostics
    open FSharp.Control.Tasks.NonAffine
    open Newtonsoft.Json

    type CaptchaValidationResult =
        | ServerError of string
        | UserError   of string
        | Success

    type CaptchaResult =
        {
            [<JsonProperty("success")>]
            IsValid        : bool

            [<JsonProperty("challenge_ts")>]
            ChallengedTime : DateTime

            [<JsonProperty("hostname")>]
            Hostname       : string

            [<JsonProperty("error-codes")>]
            ErrorCodes     : string array
        }

    let private parseError (errorCode : string) =
        match errorCode with
        | "missing-input-secret"    -> ServerError "The secret parameter is missing."
        | "invalid-input-secret"    -> ServerError "The secret parameter is invalid or malformed."
        | "sitekey-secret-mismatch" -> ServerError "The hCaptcha siteKey and secretKey do not match."
        | "bad-request"             -> ServerError "The request to verify a captcha is malformed."
        | "missing-input-response"  -> UserError "Please verify that you're not a robot."
        | "invalid-input-response"
        | "invalid-or-already-seen-response" ->
            UserError "Verification failed. Please try again."
        | _                         -> ServerError (sprintf "Unknown error code: %s" errorCode)

    let validate (log : Log.Func) (siteKey : string) (secretKey : string) (captchaResponse : string) =
        task {
            let url = "https://hcaptcha.com/siteverify"
            let data = dict [ "siteKey",  siteKey
                              "secret",   secretKey
                              "response", captchaResponse ]

            let timer = Stopwatch.StartNew()
            let! statusCode, body = Http.postAsync url data
            timer.Stop()
            log Level.Debug (sprintf "Validated captcha in %s" (timer.Elapsed.ToMs()))

            return
                if not (statusCode.Equals HttpStatusCode.OK)
                then ServerError body
                else
                    let result = JsonConvert.DeserializeObject<CaptchaResult> body
                    match result.IsValid with
                    | true  -> Success
                    | false -> parseError (result.ErrorCodes.[0])
        }
