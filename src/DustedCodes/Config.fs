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
            Domain          : string
            BaseUrl         : string
            ForceHttps      : bool
            HttpsPort       : int
            RequestLogging  : bool
            ErrorEndpoint   : bool
        }
        static member Load() =
            {
                Domain         = Env.varOrDefault "DOMAIN_NAME" "localhost:5000"
                BaseUrl        = Env.varOrDefault "BASE_URL" "http://localhost:5000"
                ForceHttps     = Env.InvariantCulture.typedVarOrDefault "FORCE_HTTPS" false
                HttpsPort      = Env.InvariantCulture.typedVarOrDefault "HTTPS_PORT" 443
                RequestLogging = Env.InvariantCulture.typedVarOrDefault "ENABLE_REQUEST_LOGGING" true
                ErrorEndpoint  = Env.InvariantCulture.typedVarOrDefault "ENABLE_ERROR_ENDPOINT" false
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
    type Mail =
        {
            GcpProjectId     : string
            GcpDatastoreKind : string
            GcpPubSubTopic   : string
            Domain           : string
            Sender           : string
            Recipient        : string
        }
        static member Load() =
            {
                GcpProjectId     = Env.varOrDefault "GCP_PROJECT_ID" ""
                GcpDatastoreKind = Env.varOrDefault "GCP_DS_CONTACT_MESSAGE_KIND" ""
                GcpPubSubTopic   = Env.varOrDefault "GCP_PS_EMAILS_TOPIC" ""
                Domain           = Env.varOrDefault "MAIL_DOMAIN" ""
                Sender           = Env.varOrDefault "MAIL_SENDER" ""
                Recipient        = Env.varOrDefault "CONTACT_MESSAGES_RECIPIENT" ""
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
            Blog         : Blog
            ThirdParties : ThirdParties
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
                        "Force HTTPS", this.Web.ForceHttps.ToString()
                        "HTTPS Port", this.Web.HttpsPort.ToString()
                        "Request Logging", this.Web.RequestLogging.ToString()
                        "Error Endpoint", this.Web.ErrorEndpoint.ToString()
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
                    "Mail", dict [
                        "GCP Project ID", this.Mail.GcpProjectId
                        "GCP Datastore Kind", this.Mail.GcpDatastoreKind
                        "GCP PubSub Topic", this.Mail.GcpPubSubTopic
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
            Blog         = Blog.Load()
            ThirdParties = ThirdParties.Load()
            Mail         = Mail.Load()
            Redis        = Redis.Load()
        } : Settings

    let appRoot       = Directory.GetCurrentDirectory()
    let contentRoot   = Path.Combine(appRoot, "Content")
    let assetsPath    = Path.Combine(appRoot, "Public")
    let blogPostsPath = Path.Combine(appRoot, "BlogPosts")








