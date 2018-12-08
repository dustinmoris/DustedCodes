module DustedCodes.Views

open System
open System.Net
open Giraffe.GiraffeViewEngine
open DustedCodes.BlogPosts
open DustedCodes.Icons
open DustedCodes

// ---------------------------------
// Helper functions
// ---------------------------------

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
        })();" Config.disqusShortName id (WebUtility.UrlEncode title) url)
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

let masterView (subject   : string option)
               (permalink : string option)
               (sample    : string option)
               (headerContent : XmlNode list option)
               (bodyContent   : XmlNode list) =
    let pageTitle =
        match subject with
        | Some s -> sprintf "%s - %s" s Config.blogTitle
        | None   -> sprintf "%s - %s" Config.blogTitle Config.blogDescription
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

            if permalink.IsSome then
                // Twitter card tags
                yield twitterCard "card" "summary"
                yield twitterCard "site" "@dustinmoris"
                yield twitterCard "creator" "@dustinmoris"

                // Open Graph tags
                yield openGraph "title"        pageTitle
                yield openGraph "url"          permalink.Value
                yield openGraph "type"         "website"
                yield openGraph "image"        "https://storage.googleapis.com/dusted-codes/stock-images/img1.jpg"
                yield openGraph "image:alt"    Config.blogTitle
                yield openGraph "image:width"  "1094"
                yield openGraph "image:height" "729"
                if sample.IsSome then
                    yield openGraph "description" sample.Value

            // Minified & bundled CSS
            yield css (Url.create minifiedCss.Path)

            // Google Analytics
            if Config.isProduction then yield googleAnalytics

            // Additional (optional) header content
            if headerContent.IsSome then yield! headerContent.Value
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
            main [] bodyContent
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
                    li [] [ normalLink Url.``/about`` "About"]
                    li [] [ normalLink Url.``/hire`` "Hire" ]
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
    ] |> masterView None (Some Url.``/``) (Some Config.blogDescription) None

let trendingView (blogPosts : BlogPost list) =
    let pageTitle = "Trending"
    let h1Title   = "Top 10 blog posts"
    [
        article[] [
            header [] [
                h1 [] [ rawText h1Title ]
            ]
            main [] [
                ol [ _id "trending-list" ] [
                    yield!
                        blogPosts
                        |> List.map trendingBlogPostListItem
                ]
            ]
        ]
        disqusCountScript
    ] |> masterView
        (Some pageTitle)
        (Some Url.``/trending``)
        (Some "Most popular blog posts on Dusted Codes.")
        None

let tagView (tag : string) (blogPosts : BlogPost list) =
    let title = sprintf "Tagged with '%s'" tag
    [
        article [] [
            header [] [
                h1 [] [ encodedText title ]
            ]
            main [] [
                ul [ _id "blogpost-list" ] [
                    yield!
                        blogPosts
                        |> List.map trendingBlogPostListItem
                ]
            ]
        ]
        disqusCountScript
    ] |> masterView
        (Some title)
        (Some (Url.``/tagged/%s`` tag))
        (Some (sprintf "See all blog posts tagged with '%s'." tag))
        None

let shareBlogPostLinks (p : BlogPost) = [
    li [] [
        iconLink
            (sprintf "mailto:?subject=%s&body=I saw this on Dusted Codes and thought you should see it: %s - %s" p.Title p.Title p.UrlEncodedPermalink)
            "Share by Email" envelopeIcon
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
    ] |> masterView (Some blogPost.Title) (Some blogPost.Permalink) (Some blogPost.Excerpt) None

let aboutView =
    [
        article [] [
            img [ _id "avatar"; _src "https://storage.googleapis.com/dusted-codes/dustin-moris-gorski.jpg"; _alt "Dustin Moris Gorski" ]
            rawText About.content
        ]
    ] |> masterView
        (Some "About")
        (Some Url.``/about``)
        (Some "Hi, welcome to my personal website, software engineering blog and...")
        None

let sendMessageButton =
    button [ _type "submit"; _class "msg-button" ] [
        envelopeIcon
        span [] [ rawText "Send Message" ]
    ]

let contactForm (msg : ContactMessage) =
    let actionUrl = sprintf "%s#contact" Url.``/hire``
    form [ _method "POST"; _action actionUrl; _autocomplete "on" ]
        [
            div [ _class "linked-inputs" ] [
                div [] [
                    label [ _for "Name" ] [ rawText "Name*" ]
                    input [
                        _type "text"
                        _name "Name"
                        _placeholder "Required"
                        _value msg.Name
                    ]
                ]
                div [] [
                    label [ _for "Email" ] [ rawText "Email address*" ]
                    input [
                        _type "email"
                        _name "Email"
                        _placeholder "Required"
                        _value msg.Email
                    ]
                ]
                div [] [
                    label [ _for "Phone" ] [ rawText "Phone number" ]
                    input [
                        _type "tel"
                        _name "Phone"
                        _placeholder "Optional"
                        _value msg.Phone
                    ]
                ]
                div [] [
                    label [ _for "Subject" ] [ rawText "Subject*" ]
                    input [
                        _type "text"
                        _name "Subject"
                        _placeholder "Required"
                        _class "msg-subject"
                        _value msg.Subject
                    ]
                ]
            ]
            div [ _class "textarea-container" ] [
                label [ _for "Message" ] [ rawText "Message*" ]
                textarea [
                    _name "Message"
                    _placeholder "Required" ] [ rawText msg.Message ]
            ]
            p [ _class "footnote" ] [
                rawText "*) Fields marked with an asterisk are required."
            ]
            div [ attr "class" "form-bottom" ] [
                div [ _class "g-recaptcha"; attr "data-sitekey" Config.googleRecaptchaSiteKey ] []
                sendMessageButton
            ]
        ]

let successMsg msg = p [ _class "success-msg" ] [ encodedText msg ]
let errorMsg msg   = p [ _class "error-msg"   ] [ alertIcon; span [] [ encodedText msg ] ]

let hireView (sendMessageResult : Result<string, ContactMessage * string> option) =
    [
        article [ _id "hire" ] [
            h1 [] [ rawText "Hire Me" ]
            img [ _src "https://storage.googleapis.com/dusted-codes/stock-images/img6.jpg"; _alt "Hire Me" ]
            rawText Hire.content
        ]
        aside [ _id "contact" ] [
            yield! [
                h1 [] [ rawText "Contact Me" ]
                p [] [ rawText "Please fill out this form to send me a message and I'll get back to you soon." ]
            ]
            yield!
                match sendMessageResult with
                | Some result ->
                    match result with
                    | Ok msg           -> [ successMsg msg ]
                    | Error (obj, msg) -> [ errorMsg msg; contactForm obj ]
                | None                 -> [ contactForm ContactMessage.Empty ]
        ]
    ] |> masterView
        (Some "Hire Me")
        (Some Url.``/hire``)
        (Some (sprintf "%s..." (Hire.content.Substring(0, 288))))
        (Some [ script [ _src "https://www.google.com/recaptcha/api.js" ] [] ])

let notFoundView =
    [
        div [ _class "error-view" ] [
            h1 [] [ rawText "Page not found!" ]
            p [] [ encodedText "Sorry, the page you have requested may have been moved or deleted." ]
            p [] [ rawText "Return to the "; normalLink Url.``/`` "home page"; rawText "." ]
        ]
    ] |> masterView (Some "Page not found") None None None

let internalErrorView (errorMessage : string option) =
    [
        div [ _class "error-view" ] [
            yield h1 [] [ rawText "Whoops, an error occurred!" ]
            yield p [] [ encodedText "Sorry, there was an internal error while processing your request." ]
            yield p [] [ rawText "Please try in a little while again or return to the "; normalLink Url.``/`` "home page"; rawText "." ]
            match errorMessage with
            | Some msg -> yield p [] [ encodedText msg ]
            | None     -> ()
        ]
    ] |> masterView (Some "Internal error") None None None