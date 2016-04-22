using System.Collections.Generic;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public sealed class TrendingViewModel : BaseViewModel
    {
        public TrendingViewModel() : base("Trending")
        {
        }

        public ICollection<ArticleWrapper> Articles { get; set; }
    }
}