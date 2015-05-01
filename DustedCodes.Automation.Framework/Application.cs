using DustedCodes.Automation.Framework.Pages;

namespace DustedCodes.Automation.Framework
{
    public static class Application
    {
        public static HomePage Startup()
        {
            Driver.Init();
            Driver.Instance.Navigate().GoToUrl(AppConfig.RootUrl);

            return new HomePage();
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