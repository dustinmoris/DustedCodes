using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace DustedCodes.Core.Feeds
{
    public interface IFeedFactory
    {
        Task<SyndicationFeed> CreateRssFeed();
        Task<SyndicationFeed> CreateAtomFeed();
    }
}