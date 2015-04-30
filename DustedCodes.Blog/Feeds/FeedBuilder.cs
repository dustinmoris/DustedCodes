using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Feeds
{
    public sealed class FeedBuilder : IFeedBuilder
    {
        private readonly IFeedItemConverter _feedItemConverter;
        private string _feedTitle;
        private string _feedDescription;
        private string _feedUrl;

        public FeedBuilder(IFeedItemConverter feedItemConverter)
        {
            _feedItemConverter = feedItemConverter;
        }

        public void SetFeedTitle(string title)
        {
            _feedTitle = title;
        }

        public void SetFeedDescription(string description)
        {
            _feedDescription = description;
        }

        public void SetFeedUrl(string url)
        {
            _feedUrl = url;
        }

        public SyndicationFeed Build(IEnumerable<Article> articles)
        {
            var feedItems = articles.Select(a => _feedItemConverter.ConvertToFeedItem(a)).ToList();

            var feed = new SyndicationFeed(
                _feedTitle,
                _feedDescription,
                new Uri(_feedUrl),
                feedItems);

            return feed;
        }
    }
}