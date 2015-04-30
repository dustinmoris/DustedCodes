using System.Collections.Generic;

namespace DustedCodes.Blog.Data
{
    public interface IArticleCache
    {
        List<ArticleMetadata> Metadata { get; set; }
    }
}