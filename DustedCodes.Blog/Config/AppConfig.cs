using System.Web.Configuration;
using System.Web.Hosting;

namespace DustedCodes.Blog.Config
{
    public class AppConfig : IAppConfig
    {
        public string BlogTitle
        {
            get { return WebConfigurationManager.AppSettings["Blog_Title"]; }
        }

        public string BlogDescription
        {
            get { return WebConfigurationManager.AppSettings["Blog_Description"]; }
        }

        public int BlogPageSize
        {
            get { return int.Parse(WebConfigurationManager.AppSettings["Blog_PageSize"]); }
        }

        public string ArticlesDirectoryPath
        {
            get { return HostingEnvironment.MapPath(WebConfigurationManager.AppSettings["Articles_Directory_Path"]); }
        }

        public string DisqusShortname
        {
            get { return WebConfigurationManager.AppSettings["Disqus_Shortname"]; }
        }

        public string EditArticleUrlFormat
        {
            get { return WebConfigurationManager.AppSettings["Edit_Article_Url_Format"]; }
        }

        public bool IsProductionEnvironment
        {
            get { return bool.Parse(WebConfigurationManager.AppSettings["Is_Production_Environment"]); }
        }

        public string DateTimeFormat
        {
            get { return WebConfigurationManager.AppSettings["DateTime_Format"]; }
        }

        public string HtmlDateTimeFormat
        {
            get { return WebConfigurationManager.AppSettings["Html_DateTime_Format"]; }
        }

        public string TwitterShareUrlFormat
        {
            get { return WebConfigurationManager.AppSettings["Twitter_ShareUrl_Format"]; }
        }

        public string GooglePlusShareUrlFormat
        {
            get { return WebConfigurationManager.AppSettings["GooglePlus_ShareUrl_Format"]; }
        }

        public string FacebookShareUrlFormat
        {
            get { return WebConfigurationManager.AppSettings["Facebook_ShareUrl_Format"]; }
        }

        public string YammerShareUrlFormat
        {
            get { return WebConfigurationManager.AppSettings["Yammer_ShareUrl_Format"]; }
        }
    }
}