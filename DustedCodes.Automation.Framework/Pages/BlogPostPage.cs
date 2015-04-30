using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class BlogPostPage : BasePage
    {
        public bool IsAt(string title)
        {
            var articleTitle = Driver.Instance.FindElement(By.CssSelector("body > main > article > header > h1"));

            return articleTitle.Text == title;
        }

        public IEnumerable<string> GetTags()
        {
            var tagElements = Driver.Instance.FindElements(By.CssSelector(".tag"));

            if (tagElements != null && tagElements.Any())
                return tagElements.Select(e => e.Text);

            return null;
        }

        public HomePage GoToTag(string tag)
        {
            var xpath = string.Format("/html/body/main/article/header/descendant::a[text()='{0}']", tag);
            var tagLink = Driver.Instance.FindElement(By.XPath(xpath));

            tagLink.Click();

            return new HomePage();
        }

        public GitHubEditPage GoToEditPage()
        {
            var editLink = Driver.Instance.FindElement(By.ClassName("edit-article"));
            editLink.Click();

            return new GitHubEditPage();
        }
    }
}