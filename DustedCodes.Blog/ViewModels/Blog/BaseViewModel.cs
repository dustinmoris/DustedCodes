namespace DustedCodes.Blog.ViewModels.Blog
{
    public abstract class BaseViewModel
    {
        public string BlogTitle { get; set; }
        public string BlogDescription { get; set; }
        public string DisqusShortname { get; set; }
        public bool IsProductionEnvironment { get; set; }
    }
}