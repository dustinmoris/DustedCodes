namespace DustedCodes

[<RequireQualifiedAccess>]
module Env =
    open System
    open System.ComponentModel
    open System.Globalization

    let private strOption str =
        match String.IsNullOrEmpty str with
        | true  -> None
        | false -> Some str

    let private strSplitArray (str : string) =
        str.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

    let private tryConvertFromString<'T when 'T : struct> (cultureInfo : CultureInfo option) (value : string) =
        let culture = defaultArg cultureInfo CultureInfo.CurrentCulture
        let converter = TypeDescriptor.GetConverter (typeof<'T>)
        try Some (converter.ConvertFromString(null, culture, value) :?> 'T)
        with _ -> None

    let getVar key =
        Environment.GetEnvironmentVariable key
        |> strOption

    let varOrDefault key defaultValue =
        getVar key
        |> Option.defaultValue defaultValue

    let typedVar<'T when 'T : struct> culture key =
        Environment.GetEnvironmentVariable key
        |> strOption
        |> Option.bind (tryConvertFromString<'T> culture)

    let typedVarOrDefault<'T when 'T : struct> culture key defaultValue =
        typedVar<'T> culture key
        |> Option.defaultValue defaultValue

    let environmentVarList key =
        getVar key
        |> function
            | None   -> [||]
            | Some v -> strSplitArray v

    [<RequireQualifiedAccess>]
    module CurrentCulture =
        let typedVar<'T when 'T : struct> key =
            Environment.GetEnvironmentVariable key
            |> strOption
            |> Option.bind (tryConvertFromString<'T> (Some CultureInfo.CurrentCulture))

        let typedVarOrDefault<'T when 'T : struct> (key : string) defaultValue =
            typedVar<'T> key
            |> Option.defaultValue defaultValue

    [<RequireQualifiedAccess>]
    module InvariantCulture =
        let typedVar<'T when 'T : struct> key =
            Environment.GetEnvironmentVariable key
            |> strOption
            |> Option.bind (tryConvertFromString<'T> (Some CultureInfo.InvariantCulture))

        let typedVarOrDefault<'T when 'T : struct> (key : string) defaultValue =
            typedVar<'T> key
            |> Option.defaultValue defaultValue