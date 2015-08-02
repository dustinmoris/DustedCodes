using System.ServiceModel.Syndication;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.Feeds
{
    public interface IFeedItemConverter
    {
        SyndicationItem ConvertToFeedItem(Article article, string permalinkUrl);
    }
}