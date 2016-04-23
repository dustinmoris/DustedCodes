using System.Threading.Tasks;
using System.Web.Mvc;
using DustedCodes.Blog.ActionResults;
using DustedCodes.Core.Feeds;

namespace DustedCodes.Blog.Controllers
{
    public sealed class FeedController : Controller
    {
        private readonly IFeedFactory _feedFactory;

        public FeedController(IFeedFactory feedFactory)
        {
            _feedFactory = feedFactory;
        }

        public async Task<AtomFeedResult> Atom()
        {
            var feed = await _feedFactory.CreateAtomFeed();

            return new AtomFeedResult(feed);
        }

        public async Task<RssFeedResult> Rss()
        {
            var feed = await _feedFactory.CreateRssFeed();

            return new RssFeedResult(feed);
        }
    }
}