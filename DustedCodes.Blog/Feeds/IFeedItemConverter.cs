using System.ServiceModel.Syndication;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Feeds
{
    public interface IFeedItemConverter
    {
        SyndicationItem ConvertToFeedItem(Article article);
    }
}