using System.Linq;
using DustedCodes.Automation.Framework;
using DustedCodes.Automation.Framework.Abstractions;
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

            Footer.GoToAbout();
            Assert.IsTrue(AboutPage.IsAt());

            AboutPage.GoToAtomFeed();
            Assert.IsTrue(AtomFeed.IsAt());

            Navigation.GoToRoot();
            Navigation.GoToBlog();
            Assert.IsTrue(HomePage.IsAt());

            Navigation.GoToTrending();
            TrendingPage.IsAt();

            Navigation.GoToArchive();
            Assert.IsTrue(ArchivePage.IsAt());

            Navigation.GoToRoot();
            Navigation.GoToRssFeed();
            Assert.IsTrue(RssFeed.IsAt());
        }

        [Test]
        public void All_BlogPosts_Are_In_Correct_Order()
        {
            var blogPosts = HomePage.GetAllBlogPostsInOriginalOrder().ToList();
            var expectedBlogPosts = DataToValidate.BlogPosts.ToList();
            expectedBlogPosts.Reverse();

            for (var i = 0; i < expectedBlogPosts.Count; i++)
            {
                Assert.AreEqual(expectedBlogPosts[i].Title, blogPosts[i]);
            }
        }

        [Test]
        public void All_BlogPosts_Are_Available()
        {
            foreach (var blogPost in DataToValidate.BlogPosts)
            {
                var url = $"{AppConfig.RootUrl}{blogPost.PermalinkId}";
                Navigation.GoToUrl(url);
                
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title), $"Blog post not found: {blogPost.Title}");

                if (blogPost.Tags == null || !blogPost.Tags.Any())
                    continue;

                var tags = BlogPostPage.GetTags().ToList();
                
                Assert.IsTrue(blogPost.Tags.SequenceEqual(tags), 
                    $"Tags did not match for blog post: {blogPost.Title}, Tags: {string.Join(" ", tags)}");
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

            Assert.IsTrue(ArchivePage.GetAllBlogPosts().Contains(blogPostTitle));
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
            Footer.GoToAbout();
            AboutPage.GoToAtomFeed();

            foreach (var blogPost in DataToValidate.BlogPosts.Reverse().Take(10))
            {
                AtomFeed.GoToArticle(blogPost.Title);
                Assert.IsTrue(BlogPostPage.IsAt(blogPost.Title));
                Footer.GoToAbout();
                AboutPage.GoToAtomFeed();
            }
        }

        [Test]
        [Ignore("Removed the edit link for now")]
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
            Browser.Startup();
        }

        [TearDown]
        public void TearDown()
        {
            Browser.Quit();
        }
    }
}
