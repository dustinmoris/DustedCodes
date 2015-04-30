using System.ServiceModel.Syndication;
using DustedCodes.Blog.ActionResults;
using FizzWare.NBuilder;
using NUnit.Framework;

namespace DustedCodes.Blog.Tests
{
    [TestFixture]
    public class AtomFeedResultTests
    {
        [Test]
        public void Class_Inherits_From_FeedResult()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new AtomFeedResult(feed);

            Assert.IsInstanceOf<FeedResult>(result);
        }

        [Test]
        public void Constructor_Sets_Correct_ContentType()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new AtomFeedResult(feed);

            Assert.AreEqual("application/atom+xml", result.ContentType);
        }

        [Test]
        public void Constructor_Sets_Correct_Formatter()
        {
            var feed = Builder<SyndicationFeed>.CreateNew().Build();

            var result = new AtomFeedResult(feed);

            Assert.IsInstanceOf<Atom10FeedFormatter>(result.FeedFormatter);
            Assert.AreSame(feed, result.FeedFormatter.Feed);
        }
    }
}
