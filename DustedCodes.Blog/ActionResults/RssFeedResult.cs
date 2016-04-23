using System.ServiceModel.Syndication;

namespace DustedCodes.Blog.ActionResults
{
    public sealed class RssFeedResult : FeedResult
    {
        public RssFeedResult(SyndicationFeed feed)
            : base(new Rss20FeedFormatter(feed), "application/rss+xml")
        {
        }
    }
}