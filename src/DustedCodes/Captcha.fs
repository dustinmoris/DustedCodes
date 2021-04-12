namespace DustedCodes

[<RequireQualifiedAccess>]
module Captcha =
    open System
    open System.Net
    open System.Threading
    open System.Threading.Tasks
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

    type ValidateFunc = string -> string -> string -> CancellationToken -> Task<CaptchaValidationResult>

    let validate
        (postCaptcha        : Http.PostFormFunc)
        (siteKey            : string)
        (secretKey          : string)
        (captchaResponse    : string)
        (ct                 : CancellationToken) =
        task {
            let data = dict [ "siteKey",  siteKey
                              "secret",   secretKey
                              "response", captchaResponse ]
            let! result = postCaptcha data ct
            return
                match result with
                | Error err -> ServerError err
                | Ok json   ->
                    let captchaResult = JsonConvert.DeserializeObject<CaptchaResult> json
                    match captchaResult.IsValid with
                    | true  -> Success
                    | false -> parseError (captchaResult.ErrorCodes.[0])
        }
