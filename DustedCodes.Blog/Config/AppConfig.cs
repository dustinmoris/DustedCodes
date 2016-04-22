using System.Web.Hosting;
using WebConfig = System.Web.Configuration.WebConfigurationManager;

namespace DustedCodes.Blog.Config
{
    public sealed class AppConfig : IAppConfig
    {
        public string BlogTitle => WebConfig.AppSettings["Blog_Title"];
        public string BlogDescription => WebConfig.AppSettings["Blog_Description"];
        public int BlogPageSize => int.Parse(WebConfig.AppSettings["Blog_PageSize"]);
        public int FeedMaxItemCount => int.Parse(WebConfig.AppSettings["Feed_MaxItemCount"]);
        public string ArticlesDirectoryPath => HostingEnvironment.MapPath(WebConfig.AppSettings["Articles_Directory_Path"]);
        public string DisqusShortname => WebConfig.AppSettings["Disqus_Shortname"];
        public string EditArticleUrlFormat => WebConfig.AppSettings["Edit_Article_Url_Format"];
        public bool IsProductionEnvironment => bool.Parse(WebConfig.AppSettings["Is_Production_Environment"]);
        public bool UseCache => bool.Parse(WebConfig.AppSettings["Use_Cache"]);
        public string DateTimeFormat => WebConfig.AppSettings["DateTime_Format"];
        public string HtmlDateTimeFormat => WebConfig.AppSettings["Html_DateTime_Format"];
        public string TwitterShareUrlFormat => WebConfig.AppSettings["Twitter_ShareUrl_Format"];
        public string GooglePlusShareUrlFormat => WebConfig.AppSettings["GooglePlus_ShareUrl_Format"];
        public string FacebookShareUrlFormat => WebConfig.AppSettings["Facebook_ShareUrl_Format"];
        public string YammerShareUrlFormat => WebConfig.AppSettings["Yammer_ShareUrl_Format"];
        public string LinkedInShareUrlFormat => WebConfig.AppSettings["LinkedIn_ShareUrl_Format"];
        public string WhatsAppShareUrlFormat => WebConfig.AppSettings["WhatsApp_ShareUrl_Format"];
        public string GoogleAnalyticsViewId => WebConfig.AppSettings["GoogleAnalytics_ViewId"];
        public string GoogleAnalyticsPrivateKeyPath => WebConfig.AppSettings["GoogleAnalytics_PrivateKeyPath"];
    }
}