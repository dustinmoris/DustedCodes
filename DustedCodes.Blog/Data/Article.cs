namespace DustedCodes.Blog.Data
{
    public sealed class Article
    {
        public Article()
        {
            Metadata = new ArticleMetadata();
        }

        public ArticleMetadata Metadata { get; set; }
        public string Content { get; set; }
        public string PermalinkUrl { get; set; }
    }
}