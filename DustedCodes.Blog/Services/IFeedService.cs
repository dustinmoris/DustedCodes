using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Services
{
    public interface IFeedService
    {
        Task<SyndicationFeed> GetFeedAsync(string feedUrl, int maxItemCount);
    }
}