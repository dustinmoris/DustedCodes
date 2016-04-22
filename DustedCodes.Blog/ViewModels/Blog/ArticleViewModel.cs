namespace DustedCodes.Blog.ViewModels.Blog
{
    public sealed class ArticleViewModel : BaseViewModel
    {
        public readonly ArticleWrapper Article;

        public ArticleViewModel(ArticleWrapper articleWrapper) 
            : base(articleWrapper.Title)
        {
            Article = articleWrapper;
        }
    }
}