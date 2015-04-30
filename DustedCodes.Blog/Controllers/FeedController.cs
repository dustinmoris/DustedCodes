using System.Threading.Tasks;
using System.Web.Mvc;
using DustedCodes.Blog.ActionResults;
using DustedCodes.Blog.Helpers;
using DustedCodes.Blog.Services;

namespace DustedCodes.Blog.Controllers
{
    public class FeedController : Controller
    {
        private readonly IFeedService _feedService;
        private readonly IUrlGenerator _urlGenerator;
        private const int MaxItemCount = 10;

        public FeedController(IFeedService feedService, IUrlGenerator urlGenerator)
        {
            _feedService = feedService;
            _urlGenerator = urlGenerator;
        }

        public async Task<AtomFeedResult> Atom()
        {
            var feedUrl = _urlGenerator.GenerateAtomFeedUrl();
            var feed = await _feedService.GetFeedAsync(feedUrl, MaxItemCount);

            return new AtomFeedResult(feed);
        }

        public async Task<RssFeedResult> Rss()
        {
            var feedUrl = _urlGenerator.GenerateRssFeedUrl();
            var feed = await _feedService.GetFeedAsync(feedUrl, MaxItemCount);

            return new RssFeedResult(feed);
        }
    }
}