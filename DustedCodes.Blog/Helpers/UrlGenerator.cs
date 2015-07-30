using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DustedCodes.Blog.Helpers
{
    public sealed class UrlGenerator : IUrlGenerator
    {
        private static HttpRequest GetCurrentRequest()
        {
            return HttpContext.Current.Request;
        }

        public string GeneratePermalinkUrl(string articleId)
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action(
                "Article", 
                "Blog", 
                new RouteValueDictionary(new { id = articleId }), 
                httpRequest.Url.Scheme, 
                httpRequest.Url.Host);
        }

        public string GenerateRssFeedUrl()
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action("Rss", "Feed", null, httpRequest.Url.Scheme, httpRequest.Url.Host);
        }

        public string GenerateAtomFeedUrl()
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action("Atom", "Feed", null, httpRequest.Url.Scheme, httpRequest.Url.Host);
        }
    }
}