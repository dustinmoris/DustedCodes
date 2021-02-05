namespace DustedCodes

[<RequireQualifiedAccess>]
module Datastore =
    open System
    open System.Threading.Tasks
    open Google.Cloud.Datastore.V1
    open FSharp.Control.Tasks.NonAffine

    type SaveEntityFunc = Log.Func -> Entity -> Task<Result<string, string>>

    let toStringValue (str : string) =
        Value(StringValue = str)

    let toTextValue (str : string) =
        let v = Value(StringValue = str)
        v.ExcludeFromIndexes <- true
        v

    let toTimestampValue (dt  : DateTime) =
        let ts = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime dt
        Value(TimestampValue = ts)

    let saveEntity
        (appName    : string)
        (envName    : string)
        (entityKind : string)
        (db         : DatastoreDb) =
        let keyFactory = db.CreateKeyFactory entityKind
        fun (log : Log.Func) (entity : Entity) ->
            task {
                try
                    let key = keyFactory.CreateIncompleteKey()
                    entity.Key <- key
                    entity.["Origin"]      <- toStringValue appName
                    entity.["Environment"] <- toStringValue envName

                    let! keys = db.InsertAsync [ entity ]
                    let insertedKey = keys.[0].ToString()
                    log Level.Notice
                        (sprintf "A new entity (%s) has been successfully saved." insertedKey)
                    return Ok insertedKey
                with ex ->
                    log Level.Alert
                        (sprintf "Failed to save entity in datastore: %s\n\n%s"
                             ex.Message
                             ex.StackTrace)
                    return Error ex.Message
            }

module PubSub =
    open System.Threading.Tasks
    open System.Collections.Generic
    open Google.Protobuf
    open Google.Cloud.PubSub.V1
    open FSharp.Control.Tasks.NonAffine
    open Newtonsoft.Json

    [<CLIMutable>]
    type Message =
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

    type SendMessageFunc = Log.Func -> Message -> Task<Result<string, string>>

    let sendMessage
        (envName    : string)
        (domain     : string)
        (sender     : string)
        (recipient  : string)
        (client     : PublisherClient) =
        fun (log : Log.Func) (msg : Message) ->
            task {
                try
                    let data = msg.TemplateData
                    let msg =
                        { msg with
                            Domain = domain
                            Sender = sender
                            Recipients = [ recipient ]
                            TemplateData = Dictionary()
                        }

                    data.Keys
                    |> Seq.iter(fun key -> msg.TemplateData.Add(key, data.[key]))
                    msg.TemplateData.Add("environmentName", envName)

                    let data =
                        msg
                        |> JsonConvert.SerializeObject
                        |> ByteString.CopyFromUtf8
                    let pubSubMsg = PubsubMessage(Data = data)
                    pubSubMsg.Attributes.Add("encoding", "json-utf8")

                    let! messageId = client.PublishAsync(pubSubMsg)
                    log Level.Notice
                        (sprintf "A new message (%s) has been successfully sent." messageId)

                    return Ok messageId
                with ex ->
                    log Level.Alert
                        (sprintf "Failed to publish pubsub message: %s\n\n%s"
                            ex.Message
                            ex.StackTrace)
                    return Error ex.Message
            }

[<RequireQualifiedAccess>]
module Messages =
    open System
    open System.Threading.Tasks
    open Google.Cloud.Datastore.V1
    open FSharp.Control.Tasks.NonAffine

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

        member this.ToPubSubMessage() =
            {
                Domain       = ""
                Sender       = ""
                Recipients   = []
                CC           = []
                BCC          = []
                Subject      = "Dusted Codes: A new message has been posted"
                TemplateName = "contact-message"
                TemplateData =
                    dict [
                        "msgSubject",      this.Subject
                        "msgContent",      this.Message
                        "msgDate",         DateTimeOffset.Now.ToString("u")
                        "msgSenderName",   this.Name
                        "msgSenderEmail",  this.Email
                        "msgSenderPhone",  this.Phone
                    ]
            } : PubSub.Message

        member this.ToDatastoreEntity() =
            let entity = Entity()
            entity.["Name"]        <- this.Name         |> Datastore.toStringValue
            entity.["Email"]       <- this.Email        |> Datastore.toStringValue
            entity.["Phone"]       <- this.Phone        |> Datastore.toStringValue
            entity.["Subject"]     <- this.Subject      |> Datastore.toStringValue
            entity.["Message"]     <- this.Message      |> Datastore.toTextValue
            entity.["Date"]        <- DateTime.UtcNow   |> Datastore.toTimestampValue
            entity

    type SaveFunc = Log.Func -> ContactMsg -> Task<Result<string, string>>

    let rec waitForFirstSuccess (tasks : Task<Result<string, string>> list) =
        task {
            let! task = Task.WhenAny tasks
            match task.Result with
            | Ok _    -> return task.Result
            | Error _ ->
                return!
                    tasks
                    |> List.filter(fun t -> t = task)
                    |> waitForFirstSuccess
        }

    let save
        (saveEntity : Datastore.SaveEntityFunc)
        (publishMsg : PubSub.SendMessageFunc) =
        fun (log : Log.Func) (msg : ContactMsg) ->
            task {
                let dsEntity = msg.ToDatastoreEntity()
                let dsTask = saveEntity log dsEntity

                let pubSubMsg = msg.ToPubSubMessage()
                let pubSubTask = publishMsg log pubSubMsg

                let! result =
                    waitForFirstSuccess([
                        dsTask
                        pubSubTask ])

                return
                    match result with
                    | Ok _    -> Ok "Thank you, your message has been successfully sent!"
                    | Error _ -> Error "Message could not be saved. Please try again later."
            }