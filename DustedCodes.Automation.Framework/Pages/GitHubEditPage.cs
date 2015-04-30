using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class GitHubEditPage
    {
        public bool IsAt(string title)
        {
            var filenameTextbox = Driver.Instance.FindElement(By.CssSelector("input[type=text][name=filename]"));

            return filenameTextbox.GetAttribute("value") == title;
        }
    }
}