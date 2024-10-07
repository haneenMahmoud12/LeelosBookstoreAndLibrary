using AutoMapper;
using LeelosBookstoreAndLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LeelosBookstoreAndLibrary
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            IMapper mapper = config.CreateMapper();

            MapperInstance.Mapper = mapper;
        }

       /* public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MvcApplication));
        }*/
    }

    public static class MapperInstance
    {
        public static IMapper Mapper { get; set; }
    }
}
