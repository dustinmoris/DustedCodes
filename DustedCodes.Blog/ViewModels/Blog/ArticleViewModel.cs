namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArticleViewModel : BaseViewModel
    {
        public readonly ArticlePartialViewModel Article;

        public ArticleViewModel(ArticlePartialViewModel articlePartialViewModel) 
            : base(articlePartialViewModel.Title)
        {
            Article = articlePartialViewModel;
        }
    }
}