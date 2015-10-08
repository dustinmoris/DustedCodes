using System.Collections.Generic;
using System.Linq;
using DustedCodes.Automation.Framework.Extensions;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class HomePage
    {
        private static IWebElement OlderPostsLink => Driver.Instance.FindElementOrNull(By.LinkText("Older Posts"));

        public static bool IsAt()
        {
            var homeLink = Driver.Instance.FindElement(By.LinkText("DUSTED CODES"));
            return homeLink.GetParent().TagName == "h1";
        }

        public static void GoToBlogPost(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();
        }

        public static IEnumerable<string> GetAllBlogPostsInOriginalOrder()
        {
            Browser.GoToRootPage();

            var allBlogPosts = new List<string>();
            allBlogPosts.AddRange(GetCurrentBlogPosts());

            while (OlderPostsLink != null)
            {
                OlderPostsLink.Click();
                allBlogPosts.AddRange(GetCurrentBlogPosts());
            }

            return allBlogPosts;
        }

        public static IEnumerable<string> GetCurrentBlogPosts()
        {
            var articleLinkElements = Driver.Instance.FindElements(By.XPath("/html/body/main/article/header/h1/a"));
            var articleTitles = articleLinkElements.Select(e => e.Text);
            return articleTitles;
        }
    }
}
