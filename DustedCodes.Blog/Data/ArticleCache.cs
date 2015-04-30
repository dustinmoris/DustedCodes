using System;
using System.Collections.Generic;

namespace DustedCodes.Blog.Data
{
    public sealed class ArticleCache : IArticleCache
    {
        private static readonly Lazy<ArticleCache> LazyInstance = new Lazy<ArticleCache>(() => new ArticleCache());

        public static ArticleCache Instance { get { return LazyInstance.Value; } }

        private ArticleCache() { }

        public List<ArticleMetadata> Metadata { get; set; }
    }
}