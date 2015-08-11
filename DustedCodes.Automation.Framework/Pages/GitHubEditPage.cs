using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class GitHubEditPage : GitHubPage
    {
        public bool IsAt(string permalinkId)
        {
            var filenameField = Driver.Instance.FindElement(By.CssSelector("input[type=text][name=filename]"));

            var expectedFilename = $"{permalinkId}.html";

            return filenameField.GetAttribute("value") == expectedFilename;
        }
    }
}