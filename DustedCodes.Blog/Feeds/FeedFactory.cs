using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using DustedCodes.Blog.Config;
using DustedCodes.Blog.Helpers;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.Feeds
{
    public sealed class FeedFactory : IFeedFactory
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IFeedItemConverter _feedItemConverter;
        private readonly IUrlGenerator _urlGenerator;
        private readonly IAppConfig _appConfig;
        private readonly int _maxItemCount;

        public FeedFactory(
            IArticleRepository articleRepository, 
            IFeedItemConverter feedItemConverter, 
            IUrlGenerator urlGenerator, 
            IAppConfig appConfig, 
            int maxItemCount)
        {
            _articleRepository = articleRepository;
            _feedItemConverter = feedItemConverter;
            _urlGenerator = urlGenerator;
            _appConfig = appConfig;
            _maxItemCount = maxItemCount;
        }

        public async Task<SyndicationFeed> CreateRssFeed()
        {
            var feedUrl = _urlGenerator.GenerateRssFeedUrl();

            return await CreateFeed(feedUrl);
        }

        public async Task<SyndicationFeed> CreateAtomFeed()
        {
            var feedUrl = _urlGenerator.GenerateAtomFeedUrl();

            return await CreateFeed(feedUrl);
        }

        private async Task<SyndicationFeed> CreateFeed(string feedUrl)
        {
            var articles = await _articleRepository.GetAllOrderedByDateAsync();
            var mostRecentArticles = articles.Take(_maxItemCount);

            var feedItems = mostRecentArticles.Select(article =>
                _feedItemConverter.ConvertToFeedItem(
                    article,
                    _urlGenerator.GeneratePermalinkUrl(article.Metadata.Id)));

            var feed = new SyndicationFeed(
                _appConfig.BlogTitle,
                _appConfig.BlogDescription,
                new Uri(feedUrl),
                feedItems);

            return feed;
        }
    }
}