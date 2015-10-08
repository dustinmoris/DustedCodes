using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class RssFeed
    {
        public static bool IsAt()
        {
            var feedTitle = Driver.Instance.FindElement(By.CssSelector("#feedTitleText"));
            return feedTitle.Text == "Dusted Codes";
        }

        public static void GoToArticle(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();
        }
    }
}