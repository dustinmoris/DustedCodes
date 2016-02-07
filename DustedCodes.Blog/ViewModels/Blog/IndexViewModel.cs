using System.Collections.Generic;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class IndexViewModel : BaseViewModel
    {
        public override string PageTitle => $"{BlogTitle} - {BlogDescription}";

        public IEnumerable<ArticlePartialViewModel> Articles { get; set; }
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}