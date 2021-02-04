namespace DustedCodes

[<RequireQualifiedAccess>]
module DotEnv =
    open System
    open System.IO

    let private parseLine(line : string) =
        Log.debugF "Parsing: %s" line
        match line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries) with
        | args when args.Length = 2 ->
            Environment.SetEnvironmentVariable(
                args.[0],
                args.[1])
        | _ -> ()

    let load() =
        Log.info "Trying to load .env file..."
        let dir = Directory.GetCurrentDirectory()
        let filePath = Path.Combine(dir, ".env.ini")
        filePath
        |> File.Exists
        |> function
            | false -> Log.warning "No .env file found."
            | true  ->
                filePath
                |> File.ReadAllLines
                |> Seq.iter parseLine