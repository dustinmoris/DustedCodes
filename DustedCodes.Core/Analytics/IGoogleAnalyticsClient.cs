using System.Collections.Generic;
using System.Threading.Tasks;

namespace DustedCodes.Core.Analytics
{
    public interface IGoogleAnalyticsClient
    {
        Task<IEnumerable<PageResult>> GetTrendingPagesAsync(byte maxCount);
    }
}