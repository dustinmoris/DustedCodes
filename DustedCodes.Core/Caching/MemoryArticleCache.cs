using System;
using System.Collections.Generic;
using DustedCodes.Core.Data;

namespace DustedCodes.Core.Caching
{
    public sealed class MemoryArticleCache : IArticleCache
    {
        private static readonly Lazy<MemoryArticleCache> LazyInstance = new Lazy<MemoryArticleCache>(() => new MemoryArticleCache());

        public static MemoryArticleCache Instance { get { return LazyInstance.Value; } }

        private MemoryArticleCache() { }

        public List<ArticleMetadata> Metadata { get; set; }
    }
}