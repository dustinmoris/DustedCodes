using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class GitHubPage
    {
        public GitHubPage()
        {
            SignInIfRequired();
        }

        private static void SignInIfRequired()
        {
            if (Driver.Instance.FindElement(By.Id("login")) != null)
            {
                new GitHubLoginPage().Login(AppConfig.GitHubUsername, AppConfig.GitHubPassword);
            }
        }
    }
}