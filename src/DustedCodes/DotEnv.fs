namespace DustedCodes

module DotEnv =
    open System
    open System.IO

    let private parseLine(line : string) =
        Console.WriteLine (sprintf "Parsing: %s" line)
        match line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries) with
        | args when args.Length = 2 ->
            Environment.SetEnvironmentVariable(
                args.[0],
                args.[1])
        | _ -> ()

    let private load() =
        lazy (
            Console.WriteLine "Trying to load .env file..."
            let dir = Directory.GetCurrentDirectory()
            let filePath = Path.Combine(dir, ".env.ini")
            filePath
            |> File.Exists
            |> function
                | false -> Console.WriteLine "No .env file found."
                | true  ->
                    filePath
                    |> File.ReadAllLines
                    |> Seq.iter parseLine
        )

    let init = load().Value