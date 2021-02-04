namespace DustedCodes

[<RequireQualifiedAccess>]
module Views =
    open System
    open System.Net
    open Giraffe.ViewEngine

    // ---------------------------------
    // Helper functions
    // ---------------------------------

    let private normalLink (url : string) (text : string) =
        a [ _href url ] [ encodedText text ]

    let private iconLink (url : string) (title : string) (icon : XmlNode) =
        a [ _href url; _target "_blank"; _title title ] [ icon ]

    let private twitterCard (key : string) (value : string) =
        meta [ _name (sprintf "twitter:%s" key); _content value ]

    let private openGraph (key : string) (value : string) =
        meta [ attr "property" (sprintf "og:%s" key); attr "content" value ]

    let private metaCharset (value : string) = meta [ _charset value ]
    let private meta (key : string) (value : string) = meta [ _name key; _content value ]
    let private css (url : string) = link [ _rel "stylesheet"; _type "text/css"; _href url ]

    let private themeToggleScript =
        script [ _type "text/javascript" ] [
            rawText "const themeToggle = document.querySelector(\".btn-theme-toggle\");
const prefersDarkScheme = window.matchMedia(\"(prefers-color-scheme: dark)\");
themeToggle.addEventListener(\"click\", function() {
    if (prefersDarkScheme.matches) {
        document.body.classList.toggle(\"light-theme\");
    } else {
        document.body.classList.toggle(\"dark-theme\");
    }
});"
        ]

    let private googleAnalytics =
        script [] [
            rawText "!function(e,a,t,n,c,o,s){e.GoogleAnalyticsObject=c,e[c]=e[c]||function(){(e[c].q=e[c].q||[]).push(arguments)},e[c].l=1*new Date,o=a.createElement(t),s=a.getElementsByTagName(t)[0],o.async=1,o.src=\"//www.google-analytics.com/analytics.js\",s.parentNode.insertBefore(o,s)}(window,document,\"script\",0,\"ga\"),ga(\"create\",\"UA-60196288-1\",\"auto\"),ga(\"send\",\"pageview\");"
        ]

    let private disqusScript (shortname : string) (id : string) (title : string) (url : string) =
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
            })();" shortname id (WebUtility.UrlEncode title) url)
        ]

    let private disqusCountScript (shortname : string) =
        script [
            _id "dsq-count-scr"
            _src (sprintf "//%s.disqus.com/count.js" shortname)
            _async ] []

    let private disqus (shortname : string) (id : string) (title : string) (url : string) =
        aside [ _id "comments" ] [
            div [ _id "disqus_thread" ] []
            disqusScript shortname id title url
            disqusCountScript shortname
            noscript [] [ rawText "Please enable JavaScript to view comments." ]
        ]

    // ---------------------------------
    // Bundle and minify CSS
    // ---------------------------------

    let minifiedCss =
        Css.getBundledContent
            "bundle"
            [
                "CSS/fonts.css"
                "CSS/themes.css"
                "CSS/site.css"
                "CSS/mobile.css"
            ]

    // ---------------------------------
    // Views
    // ---------------------------------

    let private asciiArt = "
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

    let private masterView
        (settings       : Config.Settings)
        (subject        : string option)
        (permalink      : string option)
        (sample         : string option)
        (headerContent  : XmlNode list option)
        (bodyContent    : XmlNode list) =

        let baseUrl   = settings.Web.BaseUrl
        let createUrl = Url.create baseUrl

        let pageTitle =
            match subject with
            | Some s -> sprintf "%s - %s" s settings.Blog.Title
            | None   -> sprintf "%s - %s" settings.Blog.Title settings.Blog.Subtitle
        html [] [
            comment asciiArt
            head [] [
                // Metadata
                metaCharset "utf-8"
                meta "viewport" "width=device-width, initial-scale=1.0"
                meta "description" pageTitle

                // Title
                title [] [ encodedText pageTitle ]

                // Favicon
                link [ _rel "apple-touch-icon"; _sizes "180x180"; _href (createUrl "/apple-touch-icon.png?v=2") ]
                link [ _rel "icon"; _type "image/png"; _sizes "32x32"; _href (createUrl "/favicon-32x32.png?v=2") ]
                link [ _rel "icon"; _type "image/png"; _sizes "16x16"; _href (createUrl "/favicon-16x16.png?v=2") ]
                link [ _rel "manifest"; _href (createUrl "/manifest.json?v=2") ]
                link [ _rel "mask-icon"; _href (createUrl "/safari-pinned-tab.svg?v=2"); attr "color" "#333333" ]
                link [ _rel "shortcut icon"; _href (createUrl "/favicon.ico?v=2") ]
                meta "apple-mobile-web-app-title" settings.Blog.Title
                meta "application-name" settings.Blog.Title
                meta "theme-color" "#ffffff"

                // RSS feed
                link [
                    _rel "alternate"
                    _type "application/rss+xml"
                    _title "RSS Feed"
                    _href (Url.``/feed/rss`` baseUrl)
                ]

                if permalink.IsSome then
                    // Twitter card tags
                    twitterCard "card" "summary"
                    twitterCard "site" "@dustinmoris"
                    twitterCard "creator" "@dustinmoris"

                    // Open Graph tags
                    openGraph "title"        pageTitle
                    openGraph "url"          permalink.Value
                    openGraph "type"         "website"
                    openGraph "image"        (Url.storage "images/website/open-graph-2.jpg" baseUrl)
                    openGraph "image:alt"    settings.Blog.Title
                    openGraph "image:width"  "1094"
                    openGraph "image:height" "729"
                    if sample.IsSome then
                        openGraph "description" sample.Value

                // Minified & bundled CSS
                css (createUrl minifiedCss.Path)

                // Google Analytics
                if settings.General.IsProd then googleAnalytics

                // Additional (optional) header content
                if headerContent.IsSome then yield! headerContent.Value
            ]
            body [] [
                button [ _id "theme-toggle"; _class "btn-theme-toggle" ] [
                    Icons.lightBulb
                ]
                header [] [
                    div [ _id "inner-header" ] [
                        a [ _href (Url.``/`` baseUrl) ] [
                            hgroup [] [
                                h1 [] [
                                    span [] [ encodedText "Dusted" ]
                                    Icons.logo
                                    span [] [ encodedText "Codes" ]
                                ]
                                h2 [] [ encodedText settings.Blog.Subtitle ]
                            ]
                         ]
                    ]
                ]
                main [] bodyContent
                nav [] [
                    div [ _id "inner-nav" ] [
                        ul [ _id "social-links" ] [
                            li [] [ iconLink "https://twitter.com/dustinmoris" "Follow on Twitter" Icons.twitter ]
                            li [] [ iconLink "https://www.linkedin.com/in/dustinmoris/" "Connect on LinkedIn" Icons.linkedIn ]
                            li [] [ iconLink "https://www.instagram.com/dustedtravels/" "Follow on Instagram" Icons.instagram ]
                            li [] [ iconLink "https://www.youtube.com/channel/UCtoObQtHY0TjIXwz9Ipmq7g" "Follow on YouTube" Icons.youTube ]
                            li [] [ iconLink "https://github.com/dustinmoris" "Browse GitHub repositories" Icons.github ]
                            li [] [ iconLink "https://hub.docker.com/u/dustinmoris" "Browse Docker images" Icons.docker ]
                            li [] [ iconLink "https://stackoverflow.com/users/1693158/dustinmoris" "View StackOverflow profile" Icons.stackOverflow ]
                            li [] [ iconLink "https://www.buymeacoffee.com/dustinmoris" "Buy me a coffee" Icons.buyMeACoffee ]
                            li [] [ iconLink "https://www.paypal.me/dustinmoris" "Pay me via PayPal" Icons.payPal ]
                            li [] [ iconLink (Url.``/feed/rss`` baseUrl) "Subscribe to feed" Icons.rssFeed ]
                        ]
                        ul [ _id "nav-links" ] [
                            li [] [ normalLink (Url.``/`` baseUrl) "Home" ]
                            li [] [ normalLink (Url.``/trending`` baseUrl) "Trending" ]
                            li [] [ normalLink (Url.``/about`` baseUrl) "About"]
                            li [] [ normalLink (Url.``/hire`` baseUrl) "Hire" ]
                            li [] [ normalLink (Url.``/hire#contact`` baseUrl) "Contact" ]
                        ]
                    ]
                ]
                footer [] [
                    div [ _id "inner-footer" ] [
                        h5 [] [ rawText (sprintf "Copyright &copy; %i, %s" DateTime.Now.Year settings.Blog.Author) ]
                        p [] [
                            rawText (sprintf "All content on this website, such as text, graphics, logos and images is the property of %s." settings.Blog.Author)
                        ]
                    ]
                ]

                themeToggleScript
            ]
        ]

    let private blogPostMetadata baseUrl (blogPost : BlogPosts.Article) =
        div [ _class "blogpost-metadata" ] [
            yield div [] [
                Icons.calendar
                time [ _datetime (blogPost.PublishDate.ToString("yyyy-MM-ddTHH:mm:ss")) ] [
                    encodedText (blogPost.PublishDate.ToString("d MMM yyyy")) ] ]
            if blogPost.Tags.IsSome then
                yield div [] [
                    Icons.label
                    span [ _class "tags" ] [
                        yield!
                            blogPost.Tags.Value
                            |> List.map (fun t -> normalLink (Url.``/tagged/%s`` baseUrl t) t)
                    ] ]
            yield div [] [
                Icons.comments
                a [ _class "disqus-comment-count"
                    _href (sprintf "%s#disqus_thread" (blogPost.Permalink baseUrl))
                    attr "data-disqus-identifier" blogPost.Id ] [
                        rawText "Comments"
                    ] ]
        ]

    let private trendingBlogPostListItem baseUrl (blogPost : BlogPosts.Article) =
        li [] [
            div [] [ normalLink (blogPost.Permalink baseUrl) blogPost.Title ]
            div [] [ blogPostMetadata baseUrl blogPost ] ]

    let private blogPostGroup baseUrl (year : int, blogPosts : BlogPosts.Article list) =
        div [ _class "article-list" ] [
            h3 [] [ rawText (year.ToString()) ]
            ul [] [
                yield!
                    blogPosts
                    |> List.map (
                        fun p -> li [] [ normalLink (p.Permalink baseUrl) p.Title ])
            ]
        ]

    let index (settings : Config.Settings) (blogPosts : BlogPosts.Article list) =
        [
            yield!
                blogPosts
                |> List.groupBy(fun post -> post.PublishDate.Year)
                |> List.map (blogPostGroup settings.Web.BaseUrl)
        ] |> masterView
                 settings
                 None
                 (Some (Url.``/`` settings.Web.BaseUrl))
                 (Some settings.Blog.Title)
                 None

    let trending (settings : Config.Settings) (blogPosts : BlogPosts.Article list) =
        let pageTitle = "Trending"
        let h1Title   = "Top 10 blog posts"
        [
            article [] [
                header [] [
                    h1 [] [ rawText h1Title ]
                    img [ _src (Url.storage settings.ThirdParties.StorageBaseUrl "images/website/trending.jpg") ]
                ]
                main [] [
                    ol [ _id "trending-list" ] [
                        yield!
                            blogPosts
                            |> List.map (trendingBlogPostListItem settings.Web.BaseUrl)
                    ]
                ]
            ]
            disqusCountScript settings.ThirdParties.DisqusShortname
        ] |> masterView
            settings
            (Some pageTitle)
            (Some (Url.``/trending`` settings.Web.BaseUrl))
            (Some "Most popular blog posts on Dusted Codes.")
            None

    let tagged (settings : Config.Settings) (tag : string) (blogPosts : BlogPosts.Article list) =
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
                            |> List.map (trendingBlogPostListItem settings.Web.BaseUrl)
                    ]
                ]
            ]
            disqusCountScript settings.ThirdParties.DisqusShortname
        ] |> masterView
            settings
            (Some title)
            (Some (Url.``/tagged/%s`` settings.Web.BaseUrl tag))
            (Some (sprintf "See all blog posts tagged with '%s'." tag))
            None

    let private shareBlogPostLinks baseUrl (p : BlogPosts.Article) =
        let permalink = p.UrlEncodedPermalink baseUrl
        [
        li [] [
            iconLink
                (sprintf
                     "mailto:?subject=%s&body=I saw this on Dusted Codes and thought you should see it: %s - %s"
                     p.Title p.Title
                     permalink)
                "Share by Email"
                Icons.envelope
        ]
        li [] [
            iconLink
                (sprintf "https://twitter.com/intent/tweet?url=%s&text=%s&via=dustinmoris" permalink p.UrlEncodedTitle)
                "Share on Twitter"
                Icons.twitter
        ]
        li [] [
            iconLink
                (sprintf "https://www.facebook.com/sharer/sharer.php?u=%s" permalink)
                "Share on Facebook"
                Icons.facebook
        ]
        li [] [
            iconLink
                (sprintf
                     "https://www.linkedin.com/shareArticle?mini=true&url=%s&title=%s&source=Dusted+Codes"
                     permalink
                     p.UrlEncodedTitle)
                "Share on LinkedIn"
                Icons.linkedIn
        ]
        li [] [
            iconLink
                (sprintf "https://plus.google.com/share?url=%s" permalink)
                "Share on Google Plus" Icons.googlePlus
        ]
        li [] [
            iconLink
                (sprintf "https://www.yammer.com/messages/new?login=true&status=%s" permalink)
                "Share on Yammer"
                Icons.yammer
        ]
        li [] [
            iconLink
                (sprintf "https://tumblr.com/widgets/share/tool?canonicalUrl=%s" permalink)
                "Share on Tumblr"
                Icons.tumblr
        ]
        li [] [
            iconLink
                (sprintf "https://www.reddit.com/submit?url=%s&title=%s" permalink p.UrlEncodedTitle)
                "Share on Reddit"
                Icons.reddit
        ]
        li [] [
            iconLink
                (sprintf
                     "https://news.ycombinator.com/submitlink?u=%s&t=%s"
                     permalink
                     p.UrlEncodedTitle)
                "Share on Hacker News"
                Icons.hackerNews
        ]
        li [] [
            iconLink
                (sprintf
                     "whatsapp://send?text=I saw this on Dusted Codes and thought you should see it: %s - %s"
                     p.UrlEncodedTitle
                     permalink)
                "Share on WhatsApp"
                Icons.whatsApp
        ]
        li [] [
            iconLink
                (sprintf "https://t.me/share/url?url=%s" permalink)
                "Share on Telegram"
                Icons.telegram
        ]
    ]

    let blogPost (settings : Config.Settings) (blogPost : BlogPosts.Article) =
        let permalink = blogPost.Permalink settings.Web.BaseUrl
        [
            article [] [
                yield header [] [
                    h1 [] [ encodedText blogPost.Title ]
                    blogPostMetadata settings.Web.BaseUrl blogPost
                ]
                yield main [] [
                    blogPost.HtmlContent |> rawText
                ]
                yield footer [] [
                    ul [ _class "share-links" ] [
                        yield! shareBlogPostLinks settings.Web.BaseUrl blogPost
                    ]
                ]
                yield disqus
                          settings.ThirdParties.DisqusShortname
                          blogPost.Id
                          blogPost.Title
                          permalink
            ]
        ] |> masterView
                 settings
                 (Some blogPost.Title)
                 (Some permalink)
                 (Some blogPost.Excerpt)
                 None

    let about (settings : Config.Settings) content =
        [
            article [ _id "about" ] [
                h1 [] [ rawText "About" ]
                img [
                    _id "avatar"
                    _src (Url.storage settings.ThirdParties.StorageBaseUrl "images/avatar/dustin-moris-gorski.jpg")
                    _alt "Dustin Moris Gorski"
                ]
                rawText content
                p [] [
                    a [
                        _class "bmc-button"
                        _target "_blank"
                        _href "https://www.buymeacoffee.com/dustinmoris"
                    ] [
                        Icons.buyMeACoffee
                        span [] [ rawText "Buy me a coffee" ]
                    ]
                ]
            ]
        ] |> masterView
            settings
            (Some "About")
            (Some (Url.``/about`` settings.Web.BaseUrl))
            (Some "Hi, welcome to my personal website, software engineering blog and...")
            None

    let private sendMessageButton =
        button [ _type "submit" ] [
            Icons.envelope
            span [] [
                rawText "Send Message"
            ]
        ]

    let private contactForm (settings : Config.Settings) (msg : Messages.ContactMsg) =
        let actionUrl = sprintf "%s#contact" (Url.``/hire`` settings.Web.BaseUrl)
        form [ _method "POST"; _action actionUrl; _autocomplete "on" ]
            [
                div [ ] [
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
                            _value msg.Subject
                        ]
                    ]
                ]
                div [ ] [
                    label [ _for "Message" ] [ rawText "Message*" ]
                    textarea [
                        _name "Message"
                        _placeholder "Required" ] [ rawText msg.Message ]
                ]
                p [ _class "footnote" ] [
                    rawText "*) Fields marked with an asterisk are required."
                ]
                div [] [
                    div [
                        _class "h-captcha"
                        attr "data-sitekey" settings.ThirdParties.CaptchaSiteKey
                        attr "data-theme" "dark" ] []
                    sendMessageButton
                ]
            ]

    let private successMsg msg = p [ _class "success-msg" ] [ Icons.envelope; span [] [ encodedText msg ] ]
    let private errorMsg msg   = p [ _class "error-msg"   ] [ Icons.alert; span [] [ encodedText msg ] ]

    let hire
        (settings    : Config.Settings)
        (msg         : Messages.ContactMsg)
        (msgResult   : Result<string, string> option)
        (content     : string) =
        [
            article [ _id "hire" ] [
                h1 [] [ rawText "Hire Me" ]
                img [
                    _src (Url.storage settings.ThirdParties.StorageBaseUrl "images/website/hire-me.jpg")
                    _alt "Hire Me"
                ]
                rawText content
            ]
            aside [ _id "contact" ] [
                yield! [
                    h1 [] [ rawText "Contact Me" ]
                    p [] [ rawText "Please fill out this form to send me a message and I'll get back to you as soon as I can." ]
                ]
                yield!
                    match msgResult with
                    | Some result ->
                        match result with
                        | Ok okMsg     -> [ successMsg okMsg; contactForm settings msg ]
                        | Error errMsg -> [ errorMsg errMsg;  contactForm settings msg ]
                    | None             -> [ contactForm settings msg ]
            ]
        ] |> masterView
            settings
            (Some "Hire Me")
            (Some (Url.``/hire`` settings.Web.BaseUrl))
            (Some (sprintf "%s..." (content.Substring(0, 288))))
            (Some [
                script [
                    _src "https://hcaptcha.com/1/api.js"
                    _async
                    _defer
                ] []
        ])

    let notFound (settings : Config.Settings) =
        [
            div [ _class "error-view" ] [
                h1 [] [ rawText "Page not found!" ]
                p [] [ encodedText "Sorry, the page you have requested may have been moved or deleted." ]
                p [] [
                    rawText "Return to the "
                    normalLink (Url.``/`` settings.Web.BaseUrl) "home page"
                    rawText "."
                ]
            ]
        ] |> masterView
                 settings
                 (Some "Page not found")
                 None None None

    let internalError (settings : Config.Settings) (errorMessage : string option) =
        [
            div [ _class "error-view" ] [
                yield h1 [] [ rawText "Whoops, an error occurred!" ]
                yield p [] [ encodedText "Sorry, there was an internal error while processing your request." ]
                yield p [] [
                    rawText "Please try in a little while again or return to the "
                    normalLink (Url.``/`` settings.Web.BaseUrl) "home page"
                    rawText "."
                ]
                match errorMessage with
                | Some msg -> yield p [] [ encodedText msg ]
                | None     -> ()
            ]
        ] |> masterView settings (Some "Internal error") None None None