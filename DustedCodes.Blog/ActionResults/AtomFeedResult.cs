using System.ServiceModel.Syndication;

namespace DustedCodes.Blog.ActionResults
{
    public class AtomFeedResult : FeedResult
    {
        public AtomFeedResult(SyndicationFeed feed) 
            : base(new Atom10FeedFormatter(feed), "application/atom+xml")
        {
        }
    }
}