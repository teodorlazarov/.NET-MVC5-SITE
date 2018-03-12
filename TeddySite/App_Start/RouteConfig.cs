using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeddySite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CommentsByDate",
                url: "Feedback/CommentsByDate/{userDate}",
                defaults: new {controller = "Feedback", action = "CommentsByDate", userDate = UrlParameter.Optional}
                );

            routes.MapRoute(
                name: "Comments",
                url: "Feedback/Index/{Username}",
                defaults: new { controller = "Feedback", action = "Index", Username = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
