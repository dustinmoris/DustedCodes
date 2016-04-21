using System.Collections.Generic;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class TrendingViewModel : BaseViewModel
    {
        public TrendingViewModel() : base("Trending")
        {
        }

        public ICollection<Article> Articles { get; set; }
    }
}