using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DustedCodes.Blog.Config;
using DustedCodes.Core.Web;

namespace DustedCodes.Blog.Helpers
{
    public sealed class UrlGenerator : IUrlGenerator
    {
        private readonly IAppConfig _appConfig;

        public UrlGenerator(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        private static HttpRequest GetCurrentRequest()
        {
            return HttpContext.Current.Request;
        }

        private string GetScheme(HttpRequest httpRequest)
        {
            return _appConfig.ForceHttps ? "https" : httpRequest.Url.Scheme;
        }

        private string GetBaseUrl()
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action("Index", "Blog", null, GetScheme(httpRequest), httpRequest.Url.Host);
        }

        public string GenerateFullQualifiedContentUrl(string relativePath)
        {
            var baseUri = new Uri(GetBaseUrl());
            return new Uri(baseUri, relativePath).AbsoluteUri;
        }

        public string GenerateContentUrl(string relativePath)
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Content(relativePath);
        }

        public string GeneratePermalinkUrl(string articleId)
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action(
                "Article", 
                "Blog", 
                new RouteValueDictionary(new { id = articleId }), 
                GetScheme(httpRequest), 
                httpRequest.Url.Host);
        }

        public string GenerateRssFeedUrl()
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action("Rss", "Feed", null, GetScheme(httpRequest), httpRequest.Url.Host);
        }

        public string GenerateAtomFeedUrl()
        {
            var httpRequest = GetCurrentRequest();
            var urlHelper = new UrlHelper(httpRequest.RequestContext, RouteTable.Routes);

            return urlHelper.Action("Atom", "Feed", null, GetScheme(httpRequest), httpRequest.Url.Host);
        }
    }
}