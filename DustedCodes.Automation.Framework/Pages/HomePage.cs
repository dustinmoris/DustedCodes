using System.Collections.Generic;
using System.Linq;
using DustedCodes.Automation.Framework.Extensions;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class HomePage : BasePage
    {
        public bool IsAt()
        {
            var homeLink = Driver.Instance.FindElement(By.LinkText("DUSTED CODES"));

            return homeLink.GetParent().TagName == "h1";
        }

        public BlogPostPage GoToBlogPost(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();

            return new BlogPostPage();
        }

        public IEnumerable<string> GetCurrentBlogPosts()
        {
            var articleLinkElements = Driver.Instance.FindElements(By.XPath("/html/body/main/article/header/h1/a"));
            var articleTitles = articleLinkElements.Select(e => e.Text);

            return articleTitles;
        }
    }
}
