using System.ServiceModel.Syndication;

namespace DustedCodes.Blog.ActionResults
{
    public class RssFeedResult : FeedResult
    {
        public RssFeedResult(SyndicationFeed feed)
            : base(new Rss20FeedFormatter(feed), "application/rss+xml")
        {
        }
    }
}