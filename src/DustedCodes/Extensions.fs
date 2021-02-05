namespace DustedCodes

[<AutoOpen>]
module Extensions =
    open System
    open System.Net
    open System.Text
    open System.Collections.Generic
    open System.Security.Cryptography
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Hosting
    open Microsoft.AspNetCore.HttpOverrides
    open Microsoft.Extensions.DependencyInjection

    type TimeSpan with
        member this.ToMs() =
            sprintf "%ims (%ss)"
                (this.TotalMilliseconds |> Math.Round |> int)
                (Math.Round(this.TotalSeconds, 2).ToString("0.00"))

    let private secretMask = "******"

    type String with
        member this.ToSecret() =
            match this with
            | str when String.IsNullOrEmpty str -> ""
            | str when str.Length <= 10 -> secretMask
            | str -> str.Substring(0, str.Length / 2) + secretMask

        member this.IsNullOrEmpty() =
            String.IsNullOrEmpty this

        member this.EqualsCi(other) =
            this.Equals(other, StringComparison.InvariantCultureIgnoreCase)

        member this.ToSHA1() =
            this
            |> Encoding.UTF8.GetBytes
            |> SHA1.Create().ComputeHash
            |> Array.map (fun b -> b.ToString "x2")
            |> String.concat ""

    type IPNetwork with
        member this.ToPrettyString() =
            sprintf "%s/%s"
                (this.Prefix.ToString())
                (this.PrefixLength.ToString())

    type IPAddress with
        member this.ToPrettyString() =
            this.MapToIPv4().ToString()

    type IEnumerable<'T> with
        member this.ToPrettyString() =
            this
            |> Seq.map (fun t ->
                match box t with
                | :? IPAddress as ip -> ip.ToPrettyString()
                | :? IPNetwork as nw -> nw.ToPrettyString()
                | _ -> t.ToString())
            |> String.concat ", "

    type HttpContext with
        member this.SetLogFunc(logFunc: Log.Func) =
            this.Items.Add("logFunc", logFunc)

        member this.GetLogFunc() =
            let logFunc =
                this.Items.["logFunc"]
                |> Option.ofObj
            match logFunc with
            | Some f -> f :?> Log.Func
            | None   -> Log.write Log.consoleFormat [] (Level.Debug) ""


    type IServiceCollection with
        member this.When(predicate, svcFunc) =
            match predicate with
            | true  -> svcFunc this
            | false -> this

        member this.AddRedisCache(configuration, instance) =
            this.AddStackExchangeRedisCache(
                fun options ->
                    options.InstanceName  <- instance
                    options.Configuration <- configuration)

    type IWebHostBuilder with
        member this.ConfigureSentry(dsn, envName, appVersion) =
            match dsn with
            | None -> this
            | Some dsn ->
                this.UseSentry(
                    fun sentry ->
                        sentry.Debug            <- false
                        sentry.Environment      <- envName
                        sentry.Release          <- appVersion
                        sentry.AttachStacktrace <- true
                        sentry.Dsn              <- dsn)