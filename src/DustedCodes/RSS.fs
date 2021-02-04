namespace DustedCodes

[<RequireQualifiedAccess>]
module Feed =
    open System

    type Item =
        {
            Guid        : string
            Link        : string
            Title       : string
            Description : string
            PubDate     : string
            Author      : string
            Source      : string
            Categories  : string list option
        }
        static member Create (permalink   : string)
                             (title       : string)
                             (content     : string)
                             (publishDate : DateTimeOffset)
                             (author      : string)
                             (feedUrl     : string)
                             (categories  : string list option) =
            {
                Guid        = permalink
                Link        = permalink
                Title       = title
                Description = content
                PubDate     = publishDate.ToString("R")
                Author      = author
                Source      = feedUrl
                Categories  = categories
            }

    type Channel =
        {
            Link        : string
            Title       : string
            Description : string
            Language    : string
            Generator   : string
            Items       : Item list
        }
        static member Create (channelUrl  : string)
                             (title       : string)
                             (description : string)
                             (language    : string)
                             (generator   : string)
                             (items       : Item list) =
            {
                Link        = channelUrl
                Title       = title
                Description = description
                Language    = language
                Generator   = generator
                Items       = items
            }

[<RequireQualifiedAccess>]
module RssFeed =
    open Giraffe.ViewEngine

    let create (channel : Feed.Channel) =
        let title       = tag "title"
        let link        = tag "link"
        let description = tag "description"

        tag "rss" [ attr "version" "2.0" ] [
            tag "channel" [] [
                yield title       [] [ encodedText channel.Title ]
                yield link        [] [ encodedText channel.Link ]
                yield description [] [ encodedText channel.Description ]
                yield tag "language"  [] [ encodedText channel.Language ]
                yield tag "generator" [] [ encodedText channel.Generator ]
                yield!
                    channel.Items
                    |> List.map (fun i ->
                        tag "item" [] [
                            yield tag "guid"  [] [ encodedText i.Guid ]
                            yield link        [] [ encodedText i.Link ]
                            yield title       [] [ encodedText i.Title ]
                            yield description [] [ encodedText i.Description ]
                            yield tag "pubDate" [] [ encodedText i.PubDate ]
                            yield tag "author"  [] [ encodedText i.Author ]
                            yield tag "source"  [] [ encodedText i.Source ]
                            if i.Categories.IsNone then
                                yield!
                                    i.Categories.Value
                                    |> List.map (fun c -> tag "category" [] [ encodedText c ])
                        ]
                    )
            ]
        ]