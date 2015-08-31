using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Feeds
{
    public static class AtomFeed
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