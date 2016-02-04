using System.Collections.Generic;
using DustedCodes.Blog.Configuration;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArchiveViewModel : BaseViewModel
    {
        public ArchiveViewModel(IAppConfig appConfig, string title)
            : base(appConfig, title)
        {
            Title = title;
        }

        public string Title { get; }

        public IEnumerable<Article> Articles { get; set; }
    }
}