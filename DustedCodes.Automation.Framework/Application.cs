using System.Configuration;
using DustedCodes.Automation.Framework.Pages;

namespace DustedCodes.Automation.Framework
{
    public static class Application
    {
        public static HomePage Startup()
        {
            Driver.Init();
            Driver.Instance.Navigate().GoToUrl(RootUrl);

            return new HomePage();
        }

        public static void Quit()
        {
            if (Driver.Instance != null)
            {
                Driver.Instance.Quit();
            }
        }

        public static string RootUrl
        {
            get { return ConfigurationManager.AppSettings["Root_Url"]; }
        }
    }
}