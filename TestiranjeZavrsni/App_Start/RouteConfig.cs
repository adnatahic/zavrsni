using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestiranjeZavrsni
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Index",
                url: "Index",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Login", action = "Login" }
            );

            routes.MapRoute(
                name: "Registracija",
                url: "Registracija",
                defaults: new { controller = "Registracija", action = "Registracija" }
            );

            routes.MapRoute(
               name: "Pitanja",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Testiranje", action = "Pitanja", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           


        }
    }
}
