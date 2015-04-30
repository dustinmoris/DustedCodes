namespace DustedCodes.Blog.Helpers
{
    public interface IUrlGenerator
    {
        string GeneratePermalinkUrl(string articleId);

        string GenerateRssFeedUrl();

        string GenerateAtomFeedUrl();
    }
}