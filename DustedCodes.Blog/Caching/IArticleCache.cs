using System.Collections.Generic;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.Caching
{
    public interface IArticleCache
    {
        List<ArticleMetadata> Metadata { get; set; }
    }
}