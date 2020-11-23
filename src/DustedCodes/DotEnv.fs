namespace DustedCodes

module private DotEnv =
    open System
    open System.IO

    let private parseLine(line : string) =
        printf "Parsing: %s" line
        match line.Split('=', StringSplitOptions.RemoveEmptyEntries) with
        | args when args.Length = 2 ->
            Environment.SetEnvironmentVariable(
                args.[0],
                args.[1])
        | _ -> ()

    let private load =
        let dir = Directory.GetCurrentDirectory()
        let filePath = Path.Combine(dir, ".env")
        filePath
        |> File.Exists
        |> function
            | false -> ()
            | true ->
                filePath
                |> File.ReadAllLines
                |> Seq.iter parseLine