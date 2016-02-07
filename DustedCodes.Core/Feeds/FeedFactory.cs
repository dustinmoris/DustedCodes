using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using DustedCodes.Core.Services;
using DustedCodes.Core.Web;

namespace DustedCodes.Core.Feeds
{
    public sealed class FeedFactory : IFeedFactory
    {
        private readonly IArticleService _articleService;
        private readonly IFeedItemConverter _feedItemConverter;
        private readonly IUrlGenerator _urlGenerator;
        private readonly string _feedTitle;
        private readonly string _feedDescription;
        private readonly int _maxItemCount;

        public FeedFactory(
            IArticleService articleService, 
            IFeedItemConverter feedItemConverter, 
            IUrlGenerator urlGenerator,
            string feedTitle,
            string feedDescription,
            int maxItemCount)
        {
            _articleService = articleService;
            _feedItemConverter = feedItemConverter;
            _urlGenerator = urlGenerator;
            _feedTitle = feedTitle;
            _feedDescription = feedDescription;
            _maxItemCount = maxItemCount;
        }

        public async Task<SyndicationFeed> CreateRssFeed()
        {
            var feedUrl = _urlGenerator.GenerateRssFeedUrl();

            return await CreateFeed(feedUrl).ConfigureAwait(false);
        }

        public async Task<SyndicationFeed> CreateAtomFeed()
        {
            var feedUrl = _urlGenerator.GenerateAtomFeedUrl();

            return await CreateFeed(feedUrl).ConfigureAwait(false);
        }

        private async Task<SyndicationFeed> CreateFeed(string feedUrl)
        {
            var articles = await _articleService.GetMostRecentAsync(_maxItemCount).ConfigureAwait(false);

            var feedItems = articles.Select(article =>
                _feedItemConverter.ConvertToFeedItem(
                    article,
                    _urlGenerator.GeneratePermalinkUrl(article.Id)));

            var feed = new SyndicationFeed(
                _feedTitle,
                _feedDescription,
                new Uri(feedUrl),
                feedItems);

            return feed;
        }
    }
}