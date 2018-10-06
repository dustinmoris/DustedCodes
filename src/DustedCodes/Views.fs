module DustedCodes.Views

open System
open System.Text
open System.Text.RegularExpressions
open System.Security.Cryptography
open Giraffe.GiraffeViewEngine
open DustedCodes.BlogPosts
open DustedCodes.Icons
open DustedCodes

// ---------------------------------
// Helper functions
// ---------------------------------

let titleFormat = sprintf "%s - %s"

let normalLink (url : string) (text : string) =
    a [ _href url ] [ encodedText text ]

let iconLink (url : string) (title : string) (icon : XmlNode) =
    a [ _href url; _target "_blank"; _title title ] [ icon ]

let twitterCard (key : string) (value : string) =
    meta [ _name (sprintf "twitter:%s" key); _content value ]

let openGraph (key : string) (value : string) =
    meta [ attr "property" (sprintf "og:%s" key); attr "content" value ]

let metaCharset (value : string) = meta [ _charset value ]
let meta (key : string) (value : string) = meta [ _name key; _content value ]
let css (url : string) = link [ _rel "stylesheet"; _type "text/css"; _href url ]

let googleAnalytics =
    script [] [
        rawText "!function(e,a,t,n,c,o,s){e.GoogleAnalyticsObject=c,e[c]=e[c]||function(){(e[c].q=e[c].q||[]).push(arguments)},e[c].l=1*new Date,o=a.createElement(t),s=a.getElementsByTagName(t)[0],o.async=1,o.src=\"//www.google-analytics.com/analytics.js\",s.parentNode.insertBefore(o,s)}(window,document,\"script\",0,\"ga\"),ga(\"create\",\"UA-60196288-1\",\"auto\"),ga(\"send\",\"pageview\");"
    ]

let disqusScript (id : string) (title : string) (url : string) =
    script [ _type "text/javascript"] [
        rawText (sprintf "var disqus_shortname = '%s';
        var disqus_identifier = '%s';
        var disqus_title = '%s';
        var disqus_url = '%s';

        (function () {
            var dsq = document.createElement('script');
            dsq.async = true;
            dsq.type = 'text/javascript';
            dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
            (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
        })();" Config.disqusShortName id title url)
    ]

let disqusCountScript =
    script [
        _id "dsq-count-scr"
        _src (sprintf "//%s.disqus.com/count.js" Config.disqusShortName)
        _async ] []

let disqus (id : string) (title : string) (url : string) =
    aside [ _id "comments" ] [
        div [ _id "disqus_thread" ] []
        disqusScript id title url
        disqusCountScript
        noscript [] [ rawText "Please enable JavaScript to view comments." ]
    ]

// ---------------------------------
// Bundle and minify CSS
// ---------------------------------

let minifiedCss =
    Css.getBundledContent
        [ "StyleSheets/fonts.css"; "StyleSheets/site.css" ]

// ---------------------------------
// Views
// ---------------------------------

let asciiArt = "
      _                 _                _                          _
   __| |  _   _   ___  | |_    ___    __| |       ___    ___     __| |   ___   ___
  / _` | | | | | / __| | __|  / _ \\  / _` |      / __|  / _ \\   / _` |  / _ \\ / __|
 | (_| | | |_| | \\__ \\ | |_  |  __/ | (_| |  _  | (__  | (_) | | (_| | |  __/ \\__ \\
  \\__,_|  \\__,_| |___/  \\__|  \\___|  \\__,_| (_)  \\___|  \\___/   \\__,_|  \\___| |___/

    Thank you for your visit and your interest in dusted.codes!

    Did you know that this blog is open source on GitHub?

    Whatever you are looking for, it might be easier to find it by browsing the original source code on:

    https://github.com/dustinmoris/dustedcodes

"

let baseMasterView (pageTitle : string) (openGraphUrl : string option) (content : XmlNode list) =
    html [] [
        comment asciiArt
        head [] [
            // Metadata
            yield metaCharset "utf-8"
            yield meta "viewport" "width=device-width, initial-scale=1.0"
            yield meta "description" pageTitle

            // Title
            yield title [] [ encodedText pageTitle ]

            // Favicon
            yield link [ _rel "apple-touch-icon"; _sizes "180x180"; _href (Url.create "/apple-touch-icon.png?v=2") ]
            yield link [ _rel "icon"; _type "image/png"; _sizes "32x32"; _href (Url.create "/favicon-32x32.png?v=2") ]
            yield link [ _rel "icon"; _type "image/png"; _sizes "16x16"; _href (Url.create "/favicon-16x16.png?v=2") ]
            yield link [ _rel "manifest"; _href (Url.create "/manifest.json?v=2") ]
            yield link [ _rel "mask-icon"; _href (Url.create "/safari-pinned-tab.svg?v=2"); attr "color" "#333333" ]
            yield link [ _rel "shortcut icon"; _href (Url.create "/favicon.ico?v=2") ]
            yield meta "apple-mobile-web-app-title" Config.blogTitle
            yield meta "application-name" Config.blogTitle
            yield meta "theme-color" "#ffffff"

            // RSS feed
            yield link [ _rel "alternate"; _type "application/rss+xml"; _title "RSS Feed"; _href Url.``/feed/rss`` ]

            if openGraphUrl.IsSome then
                // Twitter card tags
                yield twitterCard "card" "summary"
                yield twitterCard "site" "@dustinmoris"
                yield twitterCard "creator" "@dustinmoris"

                // Open Graph tags
                yield openGraph "title"        pageTitle
                yield openGraph "url"          openGraphUrl.Value
                yield openGraph "type"         "website"
                yield openGraph "image"        Url.``/logo.svg``
                yield openGraph "image:alt"    Config.blogTitle
                yield openGraph "image:width"  "600"
                yield openGraph "image:height" "600"

            // Minified & bundled CSS
            yield css (Url.create minifiedCss.Path)

            // Google Analytics
            if Config.isProduction then yield googleAnalytics
        ]
        body [] [
            header [] [
                div [ _id "inner-header" ] [
                    a [ _href Url.``/`` ] [
                        dustedCodesIcon
                        hgroup [] [
                            h1 [] [ encodedText Config.blogTitle ]
                            h2 [] [ encodedText Config.blogDescription ]
                        ]
                     ]
                ]
            ]
            main [] content
            nav [] [
                ul [ _id "social-links" ] [
                    li [] [ iconLink "https://www.facebook.com/dustinmoris.gorski" "Connect on Facebook" facebookIcon ]
                    li [] [ iconLink "https://twitter.com/dustinmoris" "Follow on Twitter" twitterIcon ]
                    li [] [ iconLink "https://www.linkedin.com/in/dustinmoris/" "Connect on LinkedIn" linkedInIcon ]
                    li [] [ iconLink "https://www.instagram.com/dustedtravels/" "Follow on Instagram" instagramIcon ]
                    li [] [ iconLink "https://www.youtube.com/channel/UCtoObQtHY0TjIXwz9Ipmq7g" "Follow on YouTube" youTubeIcon ]
                    li [] [ iconLink "https://github.com/dustinmoris" "Browse GitHub repositories" githubIcon ]
                    li [] [ iconLink "https://github.com/dustinmoris" "Browse Docker images" dockerIcon ]
                    li [] [ iconLink "https://stackoverflow.com/users/1693158/dustinmoris" "View StackOverflow profile" stackOverflowIcon ]
                    li [] [ iconLink "https://www.buymeacoffee.com/dustinmoris" "Buy me a coffee" buyMeACoffeeIcon ]
                    li [] [ iconLink "https://www.paypal.me/dustinmoris" "Pay me via PayPal" payPalIcon ]
                    li [] [ iconLink Url.``/feed/rss`` "Subscribe to feed" rssFeedIcon ]
                ]
                ul [ _id "nav-links" ] [
                    li [] [ normalLink Url.``/`` "Home" ]
                    li [] [ normalLink Url.``/trending`` "Trending" ]
                    // li [] [ normalLink Url.``/contact`` "Contact" ]
                    li [] [ normalLink Url.``/about`` "About"]
                ]
            ]
            footer [] [
                div [ _id "inner-footer" ] [
                    h5 [] [ rawText (sprintf "Copyright &copy; %i, %s" DateTime.Now.Year Config.blogAuthor) ]
                    p [] [
                        rawText (sprintf "All content on this website, such as text, graphics, logos and images is the property of %s." Config.blogAuthor)
                    ]
                ]
            ]
        ]
    ]

let masterView (pageTitle : string) = baseMasterView (titleFormat pageTitle Config.blogTitle)

let blogPostMetadata (blogPost : BlogPost) =
    div [ _class "blogpost-metadata" ] [
        yield div [] [
            calendarIcon
            time [ _datetime (blogPost.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss")) ] [
                encodedText (blogPost.PublishDate.ToString("d MMM yyyy")) ] ]
        if blogPost.Tags.IsSome then
            yield div [] [
                tagIcon
                span [ _class "tags" ] [
                    yield!
                        blogPost.Tags.Value
                        |> List.map (fun t -> normalLink (Url.``/tagged/%s`` t) t)
                ] ]
        yield div [] [
            commentsIcon
            a [ _class "disqus-comment-count"
                _href (sprintf "%s#disqus_thread" blogPost.Permalink)
                attr "data-disqus-identifier" blogPost.Id ] [
                    rawText "Comments"
                ] ]
    ]

let trendingBlogPostListItem (blogPost : BlogPost) =
    li [] [
        div [] [ normalLink blogPost.Permalink blogPost.Title ]
        div [] [ blogPostMetadata blogPost ] ]

let blogPostGroup (year : int, blogPosts : BlogPost list) =
    div [ _class "article-list" ] [
        h3 [] [ rawText (year.ToString()) ]
        ul [] [
            yield!
                blogPosts
                |> List.map (
                    fun p -> li [] [ normalLink p.Permalink p.Title ])
        ]
    ]

let indexView (blogPosts : BlogPost list) =
    [
        yield!
            blogPosts
            |> List.groupBy(fun post -> post.PublishDate.Year)
            |> List.map blogPostGroup
    ] |> baseMasterView (titleFormat Config.blogTitle Config.blogDescription) (Some Url.``/``)

let trendingView (blogPosts : BlogPost list) =
    let pageTitle = "Trending"
    let h1Title   = "Top 10 blog posts"
    [
        h1 [] [ rawText h1Title ]
        ol [ _id "trending-list" ] [
            yield!
                blogPosts
                |> List.map trendingBlogPostListItem
        ]
        disqusCountScript
    ] |> masterView pageTitle (Some Url.``/trending``)

let tagView (tag : string) (blogPosts : BlogPost list) =
    let title = sprintf "Tagged with '%s'" tag
    [
        h1 [] [ encodedText title ]
        ul [ _id "blogpost-list" ] [
            yield!
                blogPosts
                |> List.map trendingBlogPostListItem
        ]
    ] |> masterView title (Some (Url.``/tagged/%s`` tag))

let shareBlogPostLinks (p : BlogPost) = [
    li [] [
        iconLink
            (sprintf "mailto:?subject=%s&body=I saw this on Dusted Codes and thought you should see it: %s - %s" p.Title p.Title p.UrlEncodedPermalink)
            "Share by Email" gmailIcon
    ]
    li [] [
        iconLink
            (sprintf "https://twitter.com/intent/tweet?url=%s&text=%s&via=dustinmoris" p.UrlEncodedPermalink p.UrlEncodedTitle)
            "Share on Twitter" twitterIcon
    ]
    li [] [
        iconLink
            (sprintf "https://www.facebook.com/sharer/sharer.php?u=%s" p.UrlEncodedPermalink)
            "Share on Facebook" facebookIcon
    ]
    li [] [
        iconLink
            (sprintf "https://www.linkedin.com/shareArticle?mini=true&url=%s&title=%s&source=Dusted+Codes" p.UrlEncodedPermalink p.UrlEncodedTitle)
            "Share on LinkedIn" linkedInIcon
    ]
    li [] [
        iconLink
            (sprintf "https://plus.google.com/share?url=%s" p.UrlEncodedPermalink)
            "Share on Google Plus" googlePlusIcon
    ]
    li [] [
        iconLink
            (sprintf "https://www.yammer.com/messages/new?login=true&status=%s" p.UrlEncodedPermalink)
            "Share on Yammer" yammerIcon
    ]
    li [] [
        iconLink
            (sprintf "https://tumblr.com/widgets/share/tool?canonicalUrl=%s" p.UrlEncodedPermalink)
            "Share on Tumblr" tumblrIcon
    ]
    li [] [
        iconLink
            (sprintf "https://www.reddit.com/submit?url=%s&title=%s" p.UrlEncodedPermalink p.UrlEncodedTitle)
            "Share on Reddit" redditIcon
    ]
    li [] [
        iconLink
            (sprintf "https://news.ycombinator.com/submitlink?u=%s&t=%s" p.UrlEncodedPermalink p.UrlEncodedTitle)
            "Share on Hacker News" hackerNewsIcon
    ]
    li [] [
        iconLink
            (sprintf "whatsapp://send?text=I saw this on Dusted Codes and thought you should see it: %s - %s" p.UrlEncodedTitle p.UrlEncodedPermalink)
            "Share on WhatsApp" whatsAppIcon
    ]
    li [] [
        iconLink
            (sprintf "https://t.me/share/url?url=%s" p.UrlEncodedPermalink)
            "Share on Telegram" telegramIcon
    ]
]

let blogPostView (blogPost : BlogPost) =
    [
        article [] [
            yield header [] [
                h1 [] [ encodedText blogPost.Title ]
                blogPostMetadata blogPost
            ]
            yield main [] [
                blogPost.HtmlContent |> rawText
            ]
            yield footer [] [
                ul [ _class "share-links" ] [
                    yield! shareBlogPostLinks blogPost
                ]
            ]
            yield disqus blogPost.Id blogPost.Title blogPost.Permalink
        ]
    ] |> masterView blogPost.Title (Some blogPost.Permalink)

let aboutView =
    [
        article [ _id "about" ] [
            img [ _id "avatar"; _src "https://storage.googleapis.com/dusted-codes/dustin-moris-gorski.jpg"; _alt "Dustin Moris Gorski" ]
            rawText About.content
        ]
    ] |> masterView "About" (Some Url.``/about``)

let notFoundView =
    [
        h1 [] [ rawText "Page not found!" ]
        p [] [ encodedText "Sorry, the page you have requested may have been moved or deleted." ]
        p [] [ rawText "Return to the "; normalLink Url.``/`` "home page"; rawText "." ]
    ] |> masterView "Page not found" None

let internalErrorView (errorMessage : string option) =
    [
        yield h1 [] [ rawText "Whoops, an error occurred!" ]
        yield p [] [ encodedText "Sorry, there was an internal error while processing your request." ]
        yield p [] [ rawText "Please try in a little while again or return to the "; normalLink Url.``/`` "home page"; rawText "." ]
        match errorMessage with
        | Some msg -> yield p [] [ encodedText msg ]
        | None     -> ()
    ] |> masterView "Internal error" None