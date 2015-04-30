using System.ServiceModel.Syndication;
using DustedCodes.Blog.ActionResults;
using FizzWare.NBuilder;
using NUnit.Framework;

namespace DustedCodes.Blog.Tests
{
    [TestFixture]
    public class RssFeedResultTests
    {
        [Test]
        public void Class_Inherits_From_FeedResult()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new RssFeedResult(feed);

            Assert.IsInstanceOf<FeedResult>(result);
        }

        [Test]
        public void Constructor_Sets_Correct_ContentType()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new RssFeedResult(feed);

            Assert.AreEqual("application/rss+xml", result.ContentType);
        }

        [Test]
        public void Constructor_Sets_Correct_Formatter()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new RssFeedResult(feed);

            Assert.IsInstanceOf<Rss20FeedFormatter>(result.FeedFormatter);
            Assert.AreSame(feed, result.FeedFormatter.Feed);
        }
    }
}