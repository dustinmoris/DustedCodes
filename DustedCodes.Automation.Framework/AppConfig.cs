using System.Configuration;

namespace DustedCodes.Automation.Framework
{
    public static class AppConfig
    {
        public static string RootUrl
        {
            get { return ConfigurationManager.AppSettings["Root_Url"]; }
        }

        public static string GitHubUsername
        {
            get { return ConfigurationManager.AppSettings["GitHub_Username"]; }
        }

        public static string GitHubPassword
        {
            get { return ConfigurationManager.AppSettings["GitHub_Password"]; }
        }
    }
}