using System.Linq;
using DustedCodes.Automation.Framework;
using DustedCodes.Automation.Framework.Feeds;
using DustedCodes.Automation.Framework.Pages;
using NUnit.Framework;

namespace DustedCodes.Automation.Tests
{
    [TestFixture, Category("AutomationTests")]
    public class SmokeTests
    {
        [Test]
        public void All_Pages_Are_Available()
        {
            Assert.IsTrue(HomePage.IsAt());

            Navigation.GoToAbout();
            Assert.IsTrue(AboutPage.IsAt());

            Navigation.GoToBlog();
            Assert.IsTrue(HomePage.IsAt());

            Navigation.GoToArchive();
            Assert.IsTrue(ArchivePage.IsAt());
        }

        [Test]
        public void All_BlogPosts_Are_Available()
        {
            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                HomePage.FindAndGoToBlogPost(blogPost.Title);
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title));

                if (blogPost.Tags != null && blogPost.Tags.Any())
                {
                    var tags = BlogPostPage.GetTags();
                    Assert.IsTrue(blogPost.Tags.SequenceEqual(tags));
                }

                Navigation.GoToHome();
            }
        }

        [Test]
        public void Tag_Search_Is_Working()
        {
            var currentBlogPosts = HomePage.GetCurrentBlogPosts();
            var blogPostTitle = currentBlogPosts.First();

            HomePage.GoToBlogPost(blogPostTitle);
            var tags = BlogPostPage.GetTags();
            BlogPostPage.GoToTag(tags.First());

            Assert.IsTrue(HomePage.GetCurrentBlogPosts().Contains(blogPostTitle));
        }

        [Test]
        public void Old_BlogPost_URLs_Are_Working()
        {
            var url = $"{AppConfig.RootUrl}articles/hello-world";
            Navigation.GoToUrl(url);
            
            Assert.IsTrue(BlogPostPage.IsAt("Hello World"));
        }

        [Test]
        public void Rss_Feed_Is_Working()
        {
            Navigation.GoToRssFeed();

            foreach (var blogPost in DataToValidate.BlogPosts.Reverse().Take(10))
            {
                RssFeed.GoToArticle(blogPost.Title);
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title));
                Navigation.GoToRssFeed();
            }
        }

        [Test]
        public void Atom_Feed_Is_Working()
        {
            Navigation.GoToAtomFeed();

            foreach (var blogPost in DataToValidate.BlogPosts.Reverse().Take(10))
            {
                AtomFeed.GoToArticle(blogPost.Title);
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title));
                Navigation.GoToAtomFeed();
            }
        }

        [Test]
        public void Can_Edit_Article_In_GitHub()
        {
            // Test the edit link with the second last blog post, which should be commited to GitHub already:
            var blogPost = DataToValidate.BlogPosts.ElementAt(DataToValidate.BlogPosts.Count() - 2);
            HomePage.GoToBlogPost(blogPost.Title);
            BlogPostPage.GoToEditPage();

            Assert.IsTrue(GitHubEditPage.IsAt(blogPost.PermalinkId));
        }

        [Test]
        public void Archive_Shows_All_Posts_and_Links_Are_Working()
        {
            Navigation.GoToArchive();

            var allBlogPostsInArchive = ArchivePage.GetAllBlogPosts();
            Assert.AreEqual(DataToValidate.BlogPosts.Count(), allBlogPostsInArchive.Count());

            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                ArchivePage.GoToBlogPost(blogPost.Title);
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title));
                Navigation.GoToArchive();
            }
        }

        [SetUp]
        public void SetUp()
        {
            Application.Startup();
        }

        [TearDown]
        public void TearDown()
        {
            Application.Quit();
        }
    }
}
