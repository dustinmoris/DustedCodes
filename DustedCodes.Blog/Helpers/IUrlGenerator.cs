namespace DustedCodes.Blog.Helpers
{
    public interface IUrlGenerator
    {
        string GetBaseUrl();
        string GenerateFullQualifiedContentUrl(string relativePath);
        string GenerateContentUrl(string relativePath);
        string GeneratePermalinkUrl(string articleId);
        string GenerateRssFeedUrl();
        string GenerateAtomFeedUrl();
    }
}