using System.Web.Mvc;
using System.Web.Routing;

namespace DustedCodes.Blog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DependencyConfig.Setup();

            // Disable HTTP headers to disclose ASP.NET MVC on the server
            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}
