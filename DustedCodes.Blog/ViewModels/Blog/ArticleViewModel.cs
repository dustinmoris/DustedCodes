using DustedCodes.Blog.Configuration;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArticleViewModel : BaseViewModel
    {
        public readonly ArticlePartialViewModel Article;

        public ArticleViewModel(IAppConfig appConfig, ArticlePartialViewModel articlePartialViewModel) 
            : base(appConfig, articlePartialViewModel.Title)
        {
            Article = articlePartialViewModel;
        }
    }
}