using System.Configuration;

namespace DustedCodes.Automation.Framework
{
    public static class AppConfig
    {
        public static string RootUrl => ConfigurationManager.AppSettings["Root_Url"];
        public static string GitHubUsername => ConfigurationManager.AppSettings["GitHub_Username"];
        public static string GitHubPassword => ConfigurationManager.AppSettings["GitHub_Password"];
    }
}