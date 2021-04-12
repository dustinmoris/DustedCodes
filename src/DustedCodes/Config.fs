namespace DustedCodes

[<RequireQualifiedAccess>]
module Config =
    open System
    open System.IO
    open System.Diagnostics
    open System.Reflection
    open System.Text

    [<RequireQualifiedAccess>]
    type General =
        {
            AppName     : string
            AppVersion  : string
            LogLevel    : string
            EnvName     : string
            IsProd      : bool
        }
        static member Load() =
            let envName = Env.varOrDefault "ASPNETCORE_ENVIRONMENT" "Development"
            let appVersion =
                Assembly.GetExecutingAssembly().Location
                |> FileVersionInfo.GetVersionInfo
                |> fun v-> v.ProductVersion
            let isProduction = envName.EqualsCi "production"
            {
                AppName    = Env.varOrDefault "APP_NAME" "DustedCodes"
                AppVersion = appVersion
                LogLevel   = Env.varOrDefault "LOG_LEVEL" "debug"
                EnvName    = envName
                IsProd     = isProduction
            }

    [<RequireQualifiedAccess>]
    type Web =
        {
            Domain            : string
            BaseUrl           : string
            RequestLogging    : bool
            ErrorEndpoint     : bool
        }
        static member Load() =
            {
                Domain          = Env.varOrDefault "DOMAIN_NAME" "localhost:5000"
                BaseUrl         = Env.varOrDefault "BASE_URL" "http://localhost:5000"
                RequestLogging  = Env.InvariantCulture.typedVarOrDefault "ENABLE_REQUEST_LOGGING" true
                ErrorEndpoint   = Env.InvariantCulture.typedVarOrDefault "ENABLE_ERROR_ENDPOINT" false
            }

    [<RequireQualifiedAccess>]
    type Https =
        {
            HttpsHost         : string
            HttpsPort         : int
            ForceHttps        : bool
            BehindProxy       : bool
            PermanentRedirect : bool
        }
        static member Load() =
            {
                HttpsHost         = Env.varOrDefault "HTTPS_HOST" "localhost"
                HttpsPort         = Env.InvariantCulture.typedVarOrDefault "HTTPS_PORT" 443
                ForceHttps        = Env.InvariantCulture.typedVarOrDefault "FORCE_HTTPS" false
                BehindProxy       = Env.InvariantCulture.typedVarOrDefault "BEHIND_PROXY" false
                PermanentRedirect = Env.InvariantCulture.typedVarOrDefault "PERMANENT_REDIRECT" true
            }

    [<RequireQualifiedAccess>]
    type Proxy =
        {
            ProxyCount        : int
            FwdIPHeaderName   : string
        }
        static member Load() =
            {
                ProxyCount          = Env.InvariantCulture.typedVarOrDefault "PROXY_COUNT" 2
                FwdIPHeaderName     = Env.varOrDefault "FORWARDED_IP_HEADER_NAME" "X-Forwarded-For"
            }

    [<RequireQualifiedAccess>]
    type Blog =
        {
            Title    : string
            Subtitle : string
            Lang     : string
            Author   : string
        }
        static member Load() =
            {
                Title     = Env.varOrDefault "BLOG_TITLE"    "Dusted Codes"
                Subtitle  = Env.varOrDefault "BLOG_SUBTITLE" "Programming adventures"
                Lang      = Env.varOrDefault "BLOG_LANG"     "en-GB"
                Author    = Env.varOrDefault "BLOG_AUTHOR"   "Dustin Moris Gorski"
            }

    [<RequireQualifiedAccess>]
    type ThirdParties =
        {
            StorageBaseUrl   : string
            SentryDsn        : string option
            DisqusShortname  : string
            AnalyticsKey     : string
            AnalyticsViewId  : string
            CaptchaSiteKey   : string
            CaptchaSecretKey : string
        }
        static member Load() =
            {
                StorageBaseUrl   = Env.varOrDefault "STORAGE_BASE_URL" ""
                SentryDsn        = Env.getVar       "SENTRY_DSN"
                DisqusShortname  = Env.varOrDefault "DISQUS_SHORTNAME" ""
                AnalyticsKey     = Env.varOrDefault "GOOGLE_ANALYTICS_KEY" ""
                AnalyticsViewId  = Env.varOrDefault "GOOGLE_ANALYTICS_VIEWID" ""
                CaptchaSiteKey   = Env.varOrDefault "CAPTCHA_SITEKEY" ""
                CaptchaSecretKey = Env.varOrDefault "CAPTCHA_SECRETKEY" ""
            }

    [<RequireQualifiedAccess>]
    type GCP =
        {
            ProjectId     : string
        }
        static member Load() =
            {
                ProjectId   = Env.varOrDefault "GCP_PROJECT_ID" ""
            }

    [<RequireQualifiedAccess>]
    type Mail =
        {
            MailDropEndpoint : string
            MailDropApiKey   : string
            Domain           : string
            Sender           : string
            Recipient        : string
        }
        static member Load() =
            {
                MailDropEndpoint = Env.varOrDefault "MAIL_DROP_ENDPOINT" ""
                MailDropApiKey   = Env.varOrDefault "MAIL_DROP_API_KEY" ""
                Domain           = Env.varOrDefault "MAIL_DOMAIN" ""
                Sender           = Env.varOrDefault "MAIL_SENDER" ""
                Recipient        = Env.varOrDefault "MAIL_RECIPIENT" ""
            }

    [<RequireQualifiedAccess>]
    type Redis =
        {
            Enabled           : bool
            Configuration     : string
            Instance          : string
            CacheKeyTrending  : string
            CacheKeyPosts     : string
        }
        static member Load() =
            {
                Enabled             = Env.InvariantCulture.typedVarOrDefault "REDIS_ENABLED" false
                Configuration       = Env.varOrDefault "REDIS_CONFIGURATION" ""
                Instance            = Env.varOrDefault "REDIS_INSTANCE" "localhost"
                CacheKeyTrending    = Env.varOrDefault "CACHE_KEY_TRENDING" "trending"
                CacheKeyPosts       = Env.varOrDefault "CACHE_KEY_POSTS" "blogposts"
            }

    [<RequireQualifiedAccess>]
    type Settings =
        {
            General      : General
            Web          : Web
            Https        : Https
            Proxy        : Proxy
            Blog         : Blog
            ThirdParties : ThirdParties
            GCP          : GCP
            Mail         : Mail
            Redis        : Redis
        }
        override this.ToString() =
            let summary =
                dict [
                    "General", dict [
                        "App", this.General.AppName
                        "Version", this.General.AppVersion
                        "Log Level", this.General.LogLevel
                        "Environment", this.General.EnvName
                    ]
                    "Web", dict [
                        "Domain", this.Web.Domain
                        "Base URL", this.Web.BaseUrl
                        "Request Logging", this.Web.RequestLogging.ToString()
                        "Error Endpoint", this.Web.ErrorEndpoint.ToString()
                    ]
                    "HTTPS", dict [
                        "HTTPS Host", this.Https.HttpsHost
                        "HTTPS Port", this.Https.HttpsPort.ToString()
                        "Force HTTPS", this.Https.ForceHttps.ToString()
                        "Behind Proxy", this.Https.BehindProxy.ToString()
                        "Permanent Redirect", this.Https.PermanentRedirect.ToString()
                    ]
                    "Proxy", dict [
                        "Proxy Count", this.Proxy.ProxyCount.ToString()
                        "Forwarded IP header name", this.Proxy.FwdIPHeaderName
                    ]
                    "Blog", dict [
                        "Blog Title", this.Blog.Title
                        "Blog Subtitle", this.Blog.Subtitle
                        "Blog Language", this.Blog.Lang
                        "Blog Author", this.Blog.Author
                    ]
                    "ThirdParties", dict [
                        "Storage URL", this.ThirdParties.StorageBaseUrl
                        "Sentry DSN", (defaultArg this.ThirdParties.SentryDsn "").ToSecret()
                        "Disqus Shortname", this.ThirdParties.DisqusShortname
                        "Analytics key", this.ThirdParties.AnalyticsKey.ToSecret()
                        "Analytics viewID", this.ThirdParties.AnalyticsViewId
                        "Captcha site key", this.ThirdParties.CaptchaSiteKey
                        "Captcha secret key", this.ThirdParties.CaptchaSecretKey.ToSecret()
                    ]
                    "GCP", dict [
                        "Project ID", this.GCP.ProjectId
                    ]
                    "Mail", dict [
                        "MailDrop Endpoint", this.Mail.MailDropEndpoint
                        "MailDrop API Key", (this.Mail.MailDropApiKey.ToSecret())
                        "Mail Domain", this.Mail.Domain
                        "Mail Sender", this.Mail.Sender
                        "Mail Recipient", this.Mail.Recipient
                    ]
                    "Redis", dict [
                        "Enabled", this.Redis.Enabled.ToString()
                        "Configuration", this.Redis.Configuration
                        "Instance", this.Redis.Instance
                        "Cache Key Trending", this.Redis.CacheKeyTrending
                        "Cache Key Posts", this.Redis.CacheKeyPosts
                    ]
                ]

            let categories = summary.Keys |> Seq.toList
            let keyLength =
                categories
                |> List.fold(
                    fun (len : int) (category : string) ->
                        summary.[category].Keys
                        |> Seq.toList
                        |> List.map(fun k -> k.Length)
                        |> List.sortByDescending (id)
                        |> List.head
                        |> max len
                ) 0
            let output =
                (categories
                |> List.fold(
                    fun (sb : StringBuilder) (category : string) ->
                        summary.[category]
                        |> Seq.fold(
                            fun (sb : StringBuilder) (kvp) ->
                                let key = kvp.Key.PadLeft(keyLength, ' ')
                                let value = kvp.Value
                                sprintf "%s : %s" key value
                                |> sb.AppendLine
                        ) (sb.AppendLine("")
                             .AppendLine((sprintf "%s :" (category.ToUpper())).PadLeft(keyLength + 2, ' '))
                             .AppendLine("-----".PadRight(keyLength + 2, '-')))
                ) (StringBuilder()
                    .AppendLine("")
                    .AppendLine("")
                    .AppendLine("..:: Environment Summary ::..")))
                    .ToString()
            output

    let loadSettings() =
        {
            General      = General.Load()
            Web          = Web.Load()
            Https        = Https.Load()
            Proxy        = Proxy.Load()
            Blog         = Blog.Load()
            ThirdParties = ThirdParties.Load()
            GCP          = GCP.Load()
            Mail         = Mail.Load()
            Redis        = Redis.Load()
        } : Settings

    let appRoot       = Directory.GetCurrentDirectory()
    let contentRoot   = Path.Combine(appRoot, "Content")
    let assetsPath    = Path.Combine(appRoot, "Public")
    let blogPostsPath = Path.Combine(appRoot, "BlogPosts")








