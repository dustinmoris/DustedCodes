using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public static class BlogPostPage
    {
        public static bool IsAt(string title)
        {
            var articleTitle = Driver.Instance.FindElement(By.CssSelector("body > main > article > header > h1"));
            return articleTitle.Text == title;
        }

        public static IEnumerable<string> GetTags()
        {
            var tagElements = Driver.Instance.FindElements(By.CssSelector(".tag"));

            if (tagElements != null && tagElements.Any())
                return tagElements.Select(e => e.Text);

            return null;
        }

        public static void GoToTag(string tag)
        {
            var xpath = $"/html/body/main/article/header/descendant::a[text()='{tag}']";
            var tagLink = Driver.Instance.FindElement(By.XPath(xpath));
            tagLink.Click();
        }

        public static void GoToEditPage()
        {
            var editLink = Driver.Instance.FindElement(By.ClassName("edit-article"));
            editLink.Click();
        }
    }
}