using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using DustedCodes.Blog.Config;
using DustedCodes.Blog.Helpers;
using DustedCodes.Core.Services;

namespace DustedCodes.Blog.Feeds
{
    public sealed class FeedFactory : IFeedFactory
    {
        private readonly IArticleService _articleService;
        private readonly IFeedItemConverter _feedItemConverter;
        private readonly IUrlGenerator _urlGenerator;
        private readonly IAppConfig _appConfig;
        private readonly int _maxItemCount;

        public FeedFactory(
            IArticleService articleService, 
            IFeedItemConverter feedItemConverter, 
            IUrlGenerator urlGenerator, 
            IAppConfig appConfig, 
            int maxItemCount)
        {
            _articleService = articleService;
            _feedItemConverter = feedItemConverter;
            _urlGenerator = urlGenerator;
            _appConfig = appConfig;
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
                _appConfig.BlogTitle,
                _appConfig.BlogDescription,
                new Uri(feedUrl),
                feedItems);

            return feed;
        }
    }
}