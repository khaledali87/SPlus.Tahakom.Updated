using SPlus.API.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;




namespace SPlus.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.EnableCors();
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new DataFormatingInvoker());

            config.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            config.Formatters.Add(GlobalConfiguration.Configuration.Formatters.JsonFormatter);

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //config.MapHttpAttributeRoutes();
            //config.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            //config.Formatters.Add(GlobalConfiguration.Configuration.Formatters.JsonFormatter);
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //      routeTemplate: "{controller}/{id}",
            //    // routeTemplate: "{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


        }
    }
}
