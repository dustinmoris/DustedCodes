namespace DustedCodes.Automation.Framework
{
    public static class Application
    {
        public static void Startup()
        {
            Driver.Init();
            Driver.Instance.Navigate().GoToUrl(AppConfig.RootUrl);
        }

        public static void Quit()
        {
            if (Driver.Instance != null)
            {
                Driver.Instance.Quit();
            }
        }
    }
}