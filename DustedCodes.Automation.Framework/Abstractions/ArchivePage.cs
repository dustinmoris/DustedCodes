using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class ArchivePage
    {
        public static bool IsAt()
        {
            var title = Driver.Instance.FindElement(By.CssSelector("body > main > article > h1"));
            return title.Text == "Archive";
        }

        public static IEnumerable<string> GetAllBlogPosts()
        {
            var articleLinkElements = Driver.Instance.FindElements(By.XPath("/html/body/main/article/ul/li/a"));
            var articleTitles = articleLinkElements.Select(e => e.Text);
            return articleTitles;
        }

        public static void GoToBlogPost(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();
        }
    }
}