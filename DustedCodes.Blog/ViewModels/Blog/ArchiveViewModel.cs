using System.Collections.Generic;
using DustedCodes.Blog.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArchiveViewModel : BaseViewModel
    {
        public IEnumerable<ArticleMetadata> ArticleMetadata { get; set; }
    }
}