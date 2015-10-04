using System.Collections.Generic;
using DustedCodes.Blog.Config;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class IndexViewModel : BaseViewModel
    {
        public IndexViewModel(IAppConfig appConfig) : base(appConfig)
        {
        }

        public override string PageTitle => $"{BlogTitle} - {BlogDescription}";

        public IEnumerable<ArticlePartialViewModel> Articles { get; set; }
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}