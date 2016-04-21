namespace DustedCodes.Blog.Config
{
    public interface IAppConfig
    {
        string BlogTitle { get; }
        string BlogDescription { get; }
        int BlogPageSize { get; }
        int FeedMaxItemCount { get; }
        string ArticlesDirectoryPath { get; }
        string DisqusShortname { get; }
        string EditArticleUrlFormat { get; }
        bool IsProductionEnvironment { get; }
        bool UseCache { get; }
        string DateTimeFormat { get; }
        string HtmlDateTimeFormat { get; }
        string TwitterShareUrlFormat { get; }
        string GooglePlusShareUrlFormat { get; }
        string FacebookShareUrlFormat { get; }
        string YammerShareUrlFormat { get; }
        string LinkedInShareUrlFormat { get; }
        string WhatsAppShareUrlFormat { get; }
        string GoogleAnalyticsViewId { get; }
        string GoogleAnalyticsPrivateKeyPath { get; }
    }
}