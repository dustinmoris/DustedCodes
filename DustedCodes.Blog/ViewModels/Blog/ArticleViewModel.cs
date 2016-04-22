namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArticleViewModel : BaseViewModel
    {
        public readonly ArticleWrapper Article;

        public ArticleViewModel(ArticleWrapper articleWrapper) 
            : base(articleWrapper.Title)
        {
            Article = articleWrapper;
        }
    }
}