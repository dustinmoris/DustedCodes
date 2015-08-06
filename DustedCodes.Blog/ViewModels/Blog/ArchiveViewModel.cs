using System.Collections.Generic;
using DustedCodes.Core.Data;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArchiveViewModel : BaseViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }
}