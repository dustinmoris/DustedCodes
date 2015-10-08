namespace DustedCodes.Automation.Framework
{
    public static class Browser
    {
        public static void Startup()
        {
            Driver.Init();
            GoToRootPage();
        }

        public static void Quit()
        {
            if (Driver.Instance != null)
            {
                Driver.Instance.Quit();
            }
        }

        internal static void GoToRootPage()
        {
            GoToUrl(AppConfig.RootUrl);
        }

        internal static void GoToUrl(string url)
        {
            Driver.Instance.Navigate().GoToUrl(url);
        }
    }
}