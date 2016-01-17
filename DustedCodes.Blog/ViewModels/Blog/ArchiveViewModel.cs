using System.Collections.Generic;
using DustedCodes.Blog.Configuration;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArchiveViewModel : BaseViewModel
    {
        public ArchiveViewModel(IAppConfig appConfig) : base(appConfig, "Archive")
        {
        }

        public IEnumerable<Article> Articles { get; set; }
    }
}