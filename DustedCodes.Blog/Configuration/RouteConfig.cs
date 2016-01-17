using System.Web.Mvc;
using System.Web.Routing;

namespace DustedCodes.Blog.Configuration
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Make all URLs lower case
            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "Index",
                url: "",
                defaults: new { controller = "Blog", action = "Index", page = 1 }
            );

            routes.MapRoute(
                name: "Page",
                url: "Page/{page}",
                defaults: new { controller = "Blog", action = "Index" }
            );

            routes.MapRoute(
                name: "Archive",
                url: "Archive",
                defaults: new { controller = "Blog", action = "Archive" }
            );

            routes.MapRoute(
                name: "About",
                url: "About",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "Article",
                url: "{id}",
                defaults: new { controller = "Blog", action = "Article" }
            );

            routes.MapRoute(
                name: "Tagged",
                url: "Tagged/{tag}",
                defaults: new { controller = "Blog", action = "Tagged" }
            );

            routes.MapRoute(
                name: "RssFeed",
                url: "Feed/Rss",
                defaults: new { controller = "Feed", action = "Rss" }
            );

            routes.MapRoute(
                name: "AtomFeed",
                url: "Feed/Atom",
                defaults: new { controller = "Feed", action = "Atom" }
            );

            // Support old URLs

            routes.MapRoute(
                name: "Redirect_Old_Article_URL",
                url: "Articles/{id}",
                defaults: new { controller = "Blog", action = "ArticleRedirect" }
            );
        }
    }
}
