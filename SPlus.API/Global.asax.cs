using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using AutoMapper;

namespace SPlus.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            AutoMapper.Mapper.Initialize(cfg => 
            {
                cfg.AddProfile<UseCases.MappingProfile>();
                cfg.AddProfile<UseCases.StrategyMapper>();
                cfg.AddProfile<UseCases.ThemeMapper>();
                cfg.AddProfile<UseCases.PerspectiveMapper>();
                cfg.AddProfile<UseCases.StrategicObjectiveMapper>();      
                cfg.AddProfile<UseCases.KPIMapper>();
                cfg.AddProfile<UseCases.OrgStructureMapper>();

                cfg.AddProfile<UseCases.DivisionalObjectiveMapper>();

            });

           
        }
    }
}
