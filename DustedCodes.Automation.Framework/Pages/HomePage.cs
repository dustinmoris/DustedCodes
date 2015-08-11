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

        public BlogPostPage FindAndGoToBlogPost(string title)
        {
            var currentBlogPosts = GetCurrentBlogPosts();

            if (currentBlogPosts.Contains(title))
                return GoToBlogPost(title);

            // Totally not optimised but this does the job.
            // First navigate all the way to the end to find the blog post:
            IWebElement nextLink;
            while ((nextLink = Driver.Instance.FindElementOrNull(By.ClassName("pager-next"))) != null)
            {
                nextLink.Click();

                currentBlogPosts = GetCurrentBlogPosts();

                if (currentBlogPosts.Contains(title))
                    return GoToBlogPost(title);
            }

            // Then all the way back to the first page in case we started off somewhere in the middle
            IWebElement prevLink;
            while ((prevLink = Driver.Instance.FindElementOrNull(By.ClassName("pager-prev"))) != null)
            {
                prevLink.Click();

                currentBlogPosts = GetCurrentBlogPosts();

                if (currentBlogPosts.Contains(title))
                    return GoToBlogPost(title);
            }

            throw new NotFoundException($"The blog post with the title '{title}' cannot be found");
        }

        public IEnumerable<string> GetCurrentBlogPosts()
        {
            var articleLinkElements = Driver.Instance.FindElements(By.XPath("/html/body/main/article/header/h1/a"));
            var articleTitles = articleLinkElements.Select(e => e.Text);

            return articleTitles;
        }
    }
}
