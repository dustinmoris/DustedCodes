using DustedCodes.Automation.Framework.Pages;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Feeds
{
    public class RssFeed
    {
        public bool IsAt()
        {
            var feedTitle = Driver.Instance.FindElement(By.CssSelector("#feedTitleText"));

            return feedTitle.Text == "Dusted Codes";
        }

        public BlogPostPage GoToArticle(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();

            return new BlogPostPage();
        }
    }
}