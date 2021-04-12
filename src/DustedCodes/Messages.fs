namespace DustedCodes

[<RequireQualifiedAccess>]
module Messages =
    open System
    open System.Collections.Generic
    open System.Threading
    open System.Threading.Tasks
    open FSharp.Control.Tasks.NonAffine

    [<CLIMutable>]
    type Request =
        {
            Domain       : string
            Sender       : string
            Recipients   : string list
            CC           : string list
            BCC          : string list
            Subject      : string
            TemplateName : string
            TemplateData : IDictionary<string, string>
        }

    [<CLIMutable>]
    type ContactMsg =
        {
            Name    : string
            Email   : string
            Phone   : string
            Subject : string
            Message : string
        }

        static member Empty =
            {
                Name    = ""
                Email   = ""
                Phone   = ""
                Subject = ""
                Message = ""
            }

        member this.IsValid =
            if      String.IsNullOrEmpty this.Name    then Error "Name cannot be empty."
            else if String.IsNullOrEmpty this.Email   then Error "Email address cannot be empty."
            else if String.IsNullOrEmpty this.Subject then Error "Subject cannot be empty."
            else if String.IsNullOrEmpty this.Message then Error "Message cannot be empty."
            else Ok ()

        member this.ToRequest
            (envName    : string)
            (domain     : string)
            (sender     : string)
            (recipient  : string) =
            {
                Domain       = domain
                Sender       = sender
                Recipients   = [ recipient ]
                CC           = []
                BCC          = []
                Subject      = "Dusted Codes: A new message has been posted"
                TemplateName = "contact-message"
                TemplateData =
                    dict [
                        "environmentName", envName
                        "msgSubject",      this.Subject
                        "msgContent",      this.Message
                        "msgDate",         DateTimeOffset.Now.ToString("u")
                        "msgSenderName",   this.Name
                        "msgSenderEmail",  this.Email
                        "msgSenderPhone",  this.Phone
                    ]
            } : Request

    type SaveFunc = Log.Func -> ContactMsg -> CancellationToken -> Task<Result<string, string>>

    let save
        (postJson   : Http.PostJsonFunc)
        (envName    : string)
        (domain     : string)
        (sender     : string)
        (recipient  : string) =
        fun (log : Log.Func) (msg : ContactMsg) (ct : CancellationToken) ->
            task {
                let data = msg.ToRequest envName domain sender recipient
                let! result = postJson data ct
                return
                    match result with
                    | Ok _    -> Ok "Thank you, your message has been successfully sent!"
                    | Error err ->
                        log Level.Error (sprintf "Failed to send message to MailDrop: %s" err)
                        Error "Message could not be saved. Please try again later."
            }