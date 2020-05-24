namespace DustedCodes

[<RequireQualifiedAccess>]
module Env =
    open System
    open System.IO
    open System.Diagnostics
    open Logfella

    [<RequireQualifiedAccess>]
    module private Keys =
        let APP_NAME = "APP_NAME"
        let ENV_NAME = "ASPNETCORE_ENVIRONMENT"
        let BLOG_NAME = "BLOG_NAME"
        let BLOG_TITLE = "BLOG_TITLE"
        let BLOG_SUBTITLE = "BLOG_SUBTITLE"
        let BLOG_LANG = "BLOG_LANG"
        let BLOG_AUTHOR = "BLOG_AUTHOR"
        let LOG_LEVEL = "LOG_LEVEL"
        let SENTRY_DSN = "SENTRY_DSN"
        let DOMAIN_NAME = "DOMAIN_NAME"
        let DISQUS_SHORTNAME = "DISQUS_SHORTNAME"
        let STORAGE_BASE_URL = "STORAGE_BASE_URL"
        let MAIL_DOMAIN = "MAIL_DOMAIN"
        let MAIL_SENDER = "MAIL_SENDER"
        let CONTACT_MESSAGES_RECIPIENT = "CONTACT_MESSAGES_RECIPIENT"
        let GCP_PROJECT_ID = "GCP_PROJECT_ID"
        let GCP_DS_CONTACT_MESSAGE_KIND = "GCP_DS_CONTACT_MESSAGE_KIND"
        let GCP_PS_EMAILS_TOPIC = "GCP_PS_EMAILS_TOPIC"
        let GOOGLE_ANALYTICS_KEY = "GOOGLE_ANALYTICS_KEY"
        let GOOGLE_ANALYTICS_VIEWID = "GOOGLE_ANALYTICS_VIEWID"
        let GOOGLE_CAPTCHA_SITEKEY = "GOOGLE_CAPTCHA_SITEKEY"
        let GOOGLE_CAPTCHA_SECRETKEY = "GOOGLE_CAPTCHA_SECRETKEY"
        let ENABLE_REQUEST_LOGGING = "ENABLE_REQUEST_LOGGING"
        let ENABLE_ERROR_ENDPOINT = "ENABLE_ERROR_ENDPOINT"
        let PROXY_COUNT = "PROXY_COUNT"
        let KNOWN_PROXIES = "KNOWN_PROXIES"
        let KNOWN_PROXY_NETWORKS = "KNOWN_PROXY_NETWORKS"

    let userHomeDir = Environment.GetEnvironmentVariable "HOME"
    let defaultAppName = "DustedCodes"

    let devConfig =
        (userHomeDir, defaultAppName.ToLower())
        ||> sprintf "%s/Dusted/app-secrets/%s.json"
        |> DevConfig.load

    let appRoot = Directory.GetCurrentDirectory()
    let assetsDir = Path.Combine(appRoot, "Public")
    let blogPostsDir = Path.Combine(appRoot, "BlogPosts")
    let contentDir = Path.Combine(appRoot, "Content")

    let appName =
        Config.environmentVarOrDefault
            Keys.APP_NAME
            defaultAppName

    let appVersion =
        System.Reflection.Assembly.GetExecutingAssembly().Location
        |> FileVersionInfo.GetVersionInfo
        |> fun v-> v.ProductVersion

    let blogTitle =
        Config.environmentVarOrDefault
            Keys.BLOG_TITLE
            "Dusted Codes"

    let blogSubtitle =
        Config.environmentVarOrDefault
            Keys.BLOG_SUBTITLE
            "Programming adventures"

    let blogLanguage =
        Config.environmentVarOrDefault
            Keys.BLOG_LANG
            "en-GB"

    let blogAuthor =
        Config.environmentVarOrDefault
            Keys.BLOG_AUTHOR
            "Dustin Moris Gorski"

    let name =
        Config.environmentVarOrDefault
            Keys.ENV_NAME
            "Unknown"

    let isProduction =
        name.Equals(
            "Production",
            StringComparison.OrdinalIgnoreCase)

    let logLevel =
        Config.environmentVarOrDefault
            Keys.LOG_LEVEL
            "info"

    let logSeverity =
        logLevel.ParseSeverity()

    let sentryDsn =
        Config.environmentVarOrDefault
            Keys.SENTRY_DSN
            devConfig.[Keys.SENTRY_DSN]
        |> Str.toOption

    let domainName =
        Config.environmentVarOrDefault
            Keys.DOMAIN_NAME
            "dusted.codes"

    let baseUrl =
        match isProduction with
        | true  -> sprintf "https://%s" domainName
        | false -> "http://localhost:5000"

    let disqusShortname =
        Config.environmentVarOrDefault
            Keys.DISQUS_SHORTNAME
            devConfig.[Keys.DISQUS_SHORTNAME]

    let storageBaseUrl =
        Config.environmentVarOrDefault
            Keys.STORAGE_BASE_URL
            devConfig.[Keys.STORAGE_BASE_URL]

    let mailDomain =
        Config.environmentVarOrDefault
            Keys.MAIL_DOMAIN
            devConfig.[Keys.MAIL_DOMAIN]

    let mailSender =
        Config.environmentVarOrDefault
            Keys.MAIL_SENDER
            devConfig.[Keys.MAIL_SENDER]

    let contactMessagesRecipient =
        Config.environmentVarOrDefault
            Keys.CONTACT_MESSAGES_RECIPIENT
            devConfig.[Keys.CONTACT_MESSAGES_RECIPIENT]

    let gcpProjectId =
        Config.environmentVarOrDefault
            Keys.GCP_PROJECT_ID
            devConfig.[Keys.GCP_PROJECT_ID]

    let gcpContactMessageKind =
        Config.environmentVarOrDefault
            Keys.GCP_DS_CONTACT_MESSAGE_KIND
            devConfig.[Keys.GCP_DS_CONTACT_MESSAGE_KIND]

    let gcpContactMessageTopic =
        Config.environmentVarOrDefault
            Keys.GCP_PS_EMAILS_TOPIC
            devConfig.[Keys.GCP_PS_EMAILS_TOPIC]

    let googleAnalyticsKey =
        Config.environmentVarOrDefault
            Keys.GOOGLE_ANALYTICS_KEY
            devConfig.[Keys.GOOGLE_ANALYTICS_KEY]

    let googleAnalyticsViewId =
        Config.environmentVarOrDefault
            Keys.GOOGLE_ANALYTICS_VIEWID
            devConfig.[Keys.GOOGLE_ANALYTICS_VIEWID]

    let googleCaptchaSiteKey =
        Config.environmentVarOrDefault
            Keys.GOOGLE_CAPTCHA_SITEKEY
            devConfig.[Keys.GOOGLE_CAPTCHA_SITEKEY]

    let googleCaptchaSecretKey =
        Config.environmentVarOrDefault
            Keys.GOOGLE_CAPTCHA_SECRETKEY
            devConfig.[Keys.GOOGLE_CAPTCHA_SECRETKEY]

    let enableRequestLogging =
        Config.InvariantCulture.typedEnvironmentVarOrDefault<bool>
            Keys.ENABLE_REQUEST_LOGGING
            false

    let enableErrorEndpoint =
        Config.InvariantCulture.typedEnvironmentVarOrDefault<bool>
            Keys.ENABLE_ERROR_ENDPOINT
            (bool.Parse(devConfig.[Keys.ENABLE_ERROR_ENDPOINT]))

    let proxyCount =
        Config.InvariantCulture.typedEnvironmentVarOrDefault<int>
            Keys.PROXY_COUNT
            0

    let knownProxies =
        Keys.KNOWN_PROXIES
        |> Config.environmentVarList
        |> Array.map Network.tryParseIPAddress
        |> Array.filter Option.isSome
        |> Array.map Option.get

    let knownProxyNetworks =
        Keys.KNOWN_PROXY_NETWORKS
        |> Config.environmentVarList
        |> Array.map Network.tryParseNetworkAddress
        |> Array.filter Option.isSome
        |> Array.map Option.get

    let summary =
        dict [
            "App", dict [
                "App", appName
                "Version", appVersion
            ]
            "Blog", dict [
                "Blog Title", blogTitle
                "Blog Subtitle", blogSubtitle
                "Blog Language", blogLanguage
                "Blog Author", blogAuthor
            ]
            "Directories", dict [
                "App", appRoot
                "Assets", assetsDir
                "Blog Posts", blogPostsDir
                "Content", contentDir
            ]
            "Logging", dict [
                "Environment", name
                "Log Level", logLevel
                "Sentry DSN", sentryDsn.ToSecret()
            ]
            "URLs", dict [
                "Domain", domainName
                "Base URL", baseUrl
                "Storage URL", storageBaseUrl
                "Disqus Shortname", disqusShortname
            ]
            "Mail", dict [
                "Mail Domain", mailDomain
                "Mail Sender", mailSender
                "Contact Message Recipient", contactMessagesRecipient
            ]
            "GCP", dict [
                "Project ID", gcpProjectId
                "Messages Datastore kind", gcpContactMessageKind
                "Emails PubSub topic", gcpContactMessageTopic
                "Google Analytics key", googleAnalyticsKey.ToSecret()
                "Google Analytics viewID", googleAnalyticsViewId
                "Google Captcha site key", googleCaptchaSiteKey
                "Google Captcha secret key", googleCaptchaSecretKey.ToSecret()
            ]
            "Proxies", dict [
                "Proxy count", proxyCount.ToString()
                "Known proxies", knownProxies.ToPrettyString()
                "Known proxy networks", knownProxyNetworks.ToPrettyString()
            ]
            "Debugging", dict [
                "Request logging enabled", enableRequestLogging.ToString()
                "Error endpoint enabled", enableErrorEndpoint.ToString()
            ]
        ]