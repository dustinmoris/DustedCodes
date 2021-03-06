namespace DustedCodes

[<RequireQualifiedAccess>]
module DotEnv =
    open System
    open System.IO

    let private parseLine (log : Log.Func) (line : string) =
        log Level.Debug (sprintf "Parsing: %s" line)
        match line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries) with
        | args when args.Length = 2 ->
            Environment.SetEnvironmentVariable(
                args.[0],
                args.[1])
        | _ -> ()

    let load (log : Log.Func) =
        log Level.Info "Trying to load .env file..."
        let dir = Directory.GetCurrentDirectory()
        let filePath = Path.Combine(dir, ".env.ini")
        filePath
        |> File.Exists
        |> function
            | false -> log Level.Warning "No .env file found."
            | true  ->
                filePath
                |> File.ReadAllLines
                |> Seq.iter (parseLine log)