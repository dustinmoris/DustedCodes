using System.Web.Hosting;
using WebConfig = System.Web.Configuration.WebConfigurationManager;

namespace DustedCodes.Blog.Config
{
    public class AppConfig : IAppConfig
    {
        public string BlogTitle
        {
            get { return WebConfig.AppSettings["Blog_Title"]; }
        }

        public string BlogDescription
        {
            get { return WebConfig.AppSettings["Blog_Description"]; }
        }

        public int BlogPageSize
        {
            get { return int.Parse(WebConfig.AppSettings["Blog_PageSize"]); }
        }

        public int FeedMaxItemCount
        {
            get { return int.Parse(WebConfig.AppSettings["Feed_MaxItemCount"]); }
        }

        public string ArticlesDirectoryPath
        {
            get { return HostingEnvironment.MapPath(WebConfig.AppSettings["Articles_Directory_Path"]); }
        }

        public string DisqusShortname
        {
            get { return WebConfig.AppSettings["Disqus_Shortname"]; }
        }

        public string EditArticleUrlFormat
        {
            get { return WebConfig.AppSettings["Edit_Article_Url_Format"]; }
        }

        public bool IsProductionEnvironment
        {
            get { return bool.Parse(WebConfig.AppSettings["Is_Production_Environment"]); }
        }

        public string DateTimeFormat
        {
            get { return WebConfig.AppSettings["DateTime_Format"]; }
        }

        public string HtmlDateTimeFormat
        {
            get { return WebConfig.AppSettings["Html_DateTime_Format"]; }
        }

        public string TwitterShareUrlFormat
        {
            get { return WebConfig.AppSettings["Twitter_ShareUrl_Format"]; }
        }

        public string GooglePlusShareUrlFormat
        {
            get { return WebConfig.AppSettings["GooglePlus_ShareUrl_Format"]; }
        }

        public string FacebookShareUrlFormat
        {
            get { return WebConfig.AppSettings["Facebook_ShareUrl_Format"]; }
        }

        public string YammerShareUrlFormat
        {
            get { return WebConfig.AppSettings["Yammer_ShareUrl_Format"]; }
        }
    }
}