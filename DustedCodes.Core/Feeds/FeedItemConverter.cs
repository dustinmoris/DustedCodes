using System;
using System.Linq;
using System.ServiceModel.Syndication;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Feeds
{
    public sealed class FeedItemConverter : IFeedItemConverter
    {
        public SyndicationItem ConvertToFeedItem(Article article, string permalinkUrl)
        {
            var item = new SyndicationItem(
                article.Title,
                new TextSyndicationContent(article.Content, TextSyndicationContentKind.Html),
                new Uri(permalinkUrl),
                article.Id,
                article.PublishDateTime)
            {
                PublishDate = article.PublishDateTime
            };

            if (article.Tags == null || !article.Tags.Any())
                return item;

            foreach (var tag in article.Tags)
            {
                item.Categories.Add(new SyndicationCategory(tag));
            }

            return item;
        }
    }
}