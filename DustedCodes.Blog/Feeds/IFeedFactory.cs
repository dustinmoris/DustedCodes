using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Feeds
{
    public interface IFeedFactory
    {
        Task<SyndicationFeed> CreateRssFeed();
        Task<SyndicationFeed> CreateAtomFeed();
    }
}