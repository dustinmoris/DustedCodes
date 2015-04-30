using System.Collections.Generic;
using System.ServiceModel.Syndication;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Feeds
{
    public interface IFeedBuilder
    {
        void SetFeedTitle(string title);
        void SetFeedDescription(string description);
        void SetFeedUrl(string url);

        SyndicationFeed Build(IEnumerable<Article> articles);
    }
}