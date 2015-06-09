using System.Collections.Generic;
using DustedCodes.Blog.Data;
using DustedCodes.Blog.ViewModels.Blog;
using DustedCodes.Blog.ViewModels.Home;

namespace DustedCodes.Blog.ViewModels
{
    public interface IViewModelFactory
    {
        AboutViewModel CreateAboutViewModel();
        ArticleViewModel CreateArticleViewModel(Article article);
        IndexViewModel CreateIndexViewModel(IEnumerable<Article> articles, int totalPageCount, int currentPage);
    }
}