using System.Collections.Generic;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Caching
{
    public interface IArticleCache
    {
        List<ArticleMetadata> Metadata { get; set; }
    }
}