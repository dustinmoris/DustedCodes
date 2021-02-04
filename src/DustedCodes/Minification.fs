namespace DustedCodes

[<RequireQualifiedAccess>]
module Css =
    open System.IO
    open System.Text
    open NUglify

    type BundledCss =
        {
            Content : string
            Hash    : string
            Path    : string
        }
        static member FromContent (name: string) (content : string) =
            let hash = content.ToSHA1()
            {
                Content = content
                Hash    = hash
                Path    = sprintf "/%s.%s.css" name hash
            }

    let private getErrorMsg (errors : seq<UglifyError>) =
        let msg =
            errors
            |> Seq.fold (fun (sb : StringBuilder) t ->
                sprintf "Error: %s, File: %s" t.Message t.File
                |> sb.AppendLine
            ) (StringBuilder("Couldn't uglify content."))
        msg.ToString()

    let minify (css : string) =
        css
        |> Uglify.Css
        |> (fun res ->
            match res.HasErrors with
            | true  -> failwith (getErrorMsg res.Errors)
            | false -> res.Code)

    let getMinifiedContent (fileName : string) =
        fileName
        |> File.ReadAllText
        |> minify

    let getBundledContent (bundleName : string) (fileNames : string list) =
        let result =
            fileNames
            |> List.fold(
                fun (sb : StringBuilder) fileName ->
                    fileName
                    |> getMinifiedContent
                    |> sb.AppendLine
            ) (StringBuilder())
        result.ToString()
        |> BundledCss.FromContent bundleName