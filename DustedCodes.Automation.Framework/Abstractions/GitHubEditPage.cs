using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class GitHubEditPage
    {
        public static bool IsAt(string permalinkId)
        {
            SignInIfRequired();

            var filenameField = Driver.Instance.FindElement(By.CssSelector("input[type=text][name=filename]"));
            var expectedFilename = $"{permalinkId}.html";
            return filenameField.GetAttribute("value") == expectedFilename;
        }

        private static void SignInIfRequired()
        {
            if (Driver.Instance.FindElement(By.Id("login")) != null)
            {
                GitHubLoginPage.Login(AppConfig.GitHubUsername, AppConfig.GitHubPassword);
            }
        }
    }
}