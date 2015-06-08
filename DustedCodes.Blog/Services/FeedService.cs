using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using DustedCodes.Blog.Config;
using DustedCodes.Blog.Data;
using DustedCodes.Blog.Feeds;

namespace DustedCodes.Blog.Services
{
    public sealed class FeedService : IFeedService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IFeedBuilder _feedBuilder;
        private readonly IAppConfig _appConfig;

        public FeedService(
            IArticleRepository articleRepository, 
            IFeedBuilder feedBuilder, 
            IAppConfig appConfig)
        {
            _articleRepository = articleRepository;
            _feedBuilder = feedBuilder;
            _appConfig = appConfig;
        }

        public async Task<SyndicationFeed> GetFeedAsync(string feedUrl, int maxItemCount)
        {
            const int page = 1;

            var articles = await _articleRepository.GetMostRecentAsync(page, maxItemCount);
            
            _feedBuilder.SetFeedUrl(feedUrl);
            _feedBuilder.SetFeedTitle(_appConfig.BlogTitle);
            _feedBuilder.SetFeedDescription(_appConfig.BlogDescription);

            return _feedBuilder.Build(articles);
        }
    }
}