using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DustedCodes.Automation.Framework;
using DustedCodes.Automation.Framework.Pages;
using NUnit.Framework;

namespace DustedCodes.Automation.Tests
{
    [TestFixture, Category("SmokeTests")]
    public class SmokeTests
    {
        [Test]
        public void Home_Page_And_About_Page_Are_Available()
        {
            var homePage = Application.Startup();
            Assert.IsTrue(homePage.IsAt());

            var aboutPage = homePage.GoToAbout();
            Assert.IsTrue(aboutPage.IsAt());

            homePage = aboutPage.GoToBlog();
            Assert.IsTrue(homePage.IsAt());
        }

        [Test]
        public void All_BlogPosts_Are_Available()
        {
            var homePage = Application.Startup();

            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                var blogPostPage = homePage.FindAndGoToBlogPost(blogPost.Title);
                Assert.IsTrue(blogPostPage.IsAt(blogPost.Title));

                if (blogPost.Tags != null && blogPost.Tags.Any())
                {
                    var tags = blogPostPage.GetTags();
                    Assert.IsTrue(blogPost.Tags.SequenceEqual(tags));
                }

                homePage = blogPostPage.GoToHome();
            }
        }

        [Test]
        public void Tag_Search_Is_Working()
        {
            var homePage = Application.Startup();

            var currentBlogPosts = homePage.GetCurrentBlogPosts();
            var blogPostTitle = currentBlogPosts.First();

            var blogPostPage = homePage.GoToBlogPost(blogPostTitle);
            var tags = blogPostPage.GetTags();
            homePage = blogPostPage.GoToTag(tags.First());

            Assert.IsTrue(homePage.GetCurrentBlogPosts().Contains(blogPostTitle));
        }

        [Test]
        public void Old_BlogPost_URLs_Are_Working()
        {
            var homePage = Application.Startup();

            var url = string.Format("{0}articles/hello-world", AppConfig.RootUrl);
            homePage.GoToUrl(url);

            var blogPostPage = new BlogPostPage();
            Assert.IsTrue(blogPostPage.IsAt("Hello World"));
        }

        [Test]
        public void Rss_Feed_Is_Working()
        {
            var homePage = Application.Startup();
            var rssFeed = homePage.GoToRssFeed();

            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                var blogPostPage = rssFeed.GoToArticle(blogPost.Title);
                Assert.IsTrue(blogPostPage.IsAt(blogPost.Title));

                rssFeed = blogPostPage.GoToRssFeed();
            }
        }

        [Test]
        public void Atom_Feed_Is_Working()
        {
            var homePage = Application.Startup();
            var rssFeed = homePage.GoToAtomFeed();

            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                var blogPostPage = rssFeed.GoToArticle(blogPost.Title);
                Assert.IsTrue(blogPostPage.IsAt(blogPost.Title));

                rssFeed = blogPostPage.GoToAtomFeed();
            }
        }

        [Test]
        public async Task Not_Found_Error_Page_Is_Working()
        {
            using (var httpClient = new HttpClient())
            {
                var nonExistingUrl = string.Format("{0}this/path/does/not/exist", AppConfig.RootUrl);
                var result = await httpClient.GetAsync(new Uri(nonExistingUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
                Assert.IsTrue(content.Contains("Page not found"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }

        [Test]
        public async Task Bad_Request_Error_Page_Is_Working()
        {
            using (var httpClient = new HttpClient())
            {
                var badUrl = string.Format("{0}<script></script>", AppConfig.RootUrl);
                var result = await httpClient.GetAsync(new Uri(badUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.IsTrue(content.Contains("Bad Request"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }

        [Test]
        public async Task Forbidden_Error_Page_Is_Shown_On_NewRelic_Folder()
        {
            using (var httpClient = new HttpClient())
            {
                var badUrl = string.Format("{0}newrelic", AppConfig.RootUrl);
                var result = await httpClient.GetAsync(new Uri(badUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.IsTrue(content.Contains("Access Forbidden"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }

        [Test]
        public void Can_Edit_Article_In_GitHub()
        {
            var homePage = Application.Startup();
            // Test the edit link with the second last blog post, which should be commited to GitHub already:
            var blogPost = DataToValidate.BlogPosts.ElementAt(DataToValidate.BlogPosts.Count() - 2);
            var blogPostPage = homePage.GoToBlogPost(blogPost.Title);
            var gitHubEditPage = blogPostPage.GoToEditPage();

            Assert.IsTrue(gitHubEditPage.IsAt(blogPost.PermalinkId));
        }

        [TearDown]
        public void TearDown()
        {
            Application.Quit();
        }
    }
}
