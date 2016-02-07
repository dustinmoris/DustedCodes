using System.Collections.Generic;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArchiveViewModel : BaseViewModel
    {
        public ArchiveViewModel(string title) : base(title)
        {
            Title = title;
        }

        public string Title { get; }

        public IEnumerable<Article> Articles { get; set; }
    }
}