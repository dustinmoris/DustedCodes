using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class ArchivePage : BasePage
    {
        public bool IsAt()
        {
            var title = Driver.Instance.FindElement(By.CssSelector("body > main > article > h1"));

            return title.Text == "Archive";
        }

        public IEnumerable<string> GetAllBlogPosts()
        {
            var articleLinkElements = Driver.Instance.FindElements(By.XPath("/html/body/main/article/ul/li/a"));
            var articleTitles = articleLinkElements.Select(e => e.Text);

            return articleTitles;
        }

        public BlogPostPage GoToBlogPost(string title)
        {
            Driver.Instance.FindElement(By.LinkText(title)).Click();

            return new BlogPostPage();
        }
    }
}