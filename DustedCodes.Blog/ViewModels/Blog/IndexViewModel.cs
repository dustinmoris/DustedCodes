using System.Collections.Generic;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public sealed class IndexViewModel : BaseViewModel
    {
        public override string PageTitle => $"{BlogTitle} - {BlogDescription}";

        public IEnumerable<ArticleWrapper> Articles { get; set; }
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}