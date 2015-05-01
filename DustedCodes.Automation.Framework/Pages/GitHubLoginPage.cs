using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class GitHubLoginPage
    {
        public void Login(string username, string password)
        {
            var usernameField = Driver.Instance.FindElement(By.Id("login_field"));
            var passwordField = Driver.Instance.FindElement(By.Id("password"));
            var signInButton = Driver.Instance.FindElement(By.CssSelector("#login input[type=submit]"));

            usernameField.SendKeys(username);
            passwordField.SendKeys(password);

            signInButton.Click();
        }
    }
}