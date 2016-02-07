using System.ServiceModel.Syndication;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Feeds
{
    public interface IFeedItemConverter
    {
        SyndicationItem ConvertToFeedItem(Article article, string permalinkUrl);
    }
}