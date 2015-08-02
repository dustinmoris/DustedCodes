using System;
using System.Linq;
using System.ServiceModel.Syndication;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.Feeds
{
    public sealed class FeedItemConverter : IFeedItemConverter
    {
        public SyndicationItem ConvertToFeedItem(Article article, string permalinkUrl)
        {
            var item = new SyndicationItem(
                article.Metadata.Title,
                new TextSyndicationContent(article.Content, TextSyndicationContentKind.Html),
                new Uri(permalinkUrl),
                article.Metadata.Id,
                article.Metadata.LastEditedDateTime)
            {
                PublishDate = article.Metadata.PublishDateTime
            };

            if (article.Metadata.Tags == null || !article.Metadata.Tags.Any())
                return item;

            foreach (var tag in article.Metadata.Tags)
            {
                item.Categories.Add(new SyndicationCategory(tag));
            }

            return item;
        }
    }
}