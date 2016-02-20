namespace DustedCodes.Core.Web
{
    public interface IUrlGenerator
    {
        string GenerateFullQualifiedContentUrl(string relativePath);
        string GenerateContentUrl(string relativePath);
        string GeneratePermalinkUrl(string articleId);
        string GenerateRssFeedUrl();
        string GenerateAtomFeedUrl();
    }
}