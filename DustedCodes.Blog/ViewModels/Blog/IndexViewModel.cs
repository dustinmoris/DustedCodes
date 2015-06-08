using System.Collections.Generic;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class IndexViewModel : BaseViewModel
    {
        public IEnumerable<ArticlePartialViewModel> Articles { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
    }
}