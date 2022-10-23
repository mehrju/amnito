
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;


namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.NopStation.MobileWebApi.",
                "KeepAlive/indexApi",
                new { controller = "KeepAliveApi", action = "Index", area = "" }
           );
            routeBuilder.MapRoute("Plugin.NopStation.MobileWebApi.OpcCompleteRedirectionPayment",
                    "api/checkout/OpcCompleteRedirectionPayment",
                    new { controller = "RedirectPage", action = "OpcCompleteRedirectionPayment", area = "" }
               );

            //areas
          //  routeBuilder.MapRoute(name: "areaRoute", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            routeBuilder.MapRoute(
                name: "NopStation.MobileWebApi.LogIn",
                template: "MobileWebApi/login/{id?}",
                defaults: new { Controller = "Customer", action= "Login" }
            );
            //web api 2
            //  var config = GlobalConfiguration.Configuration;
            //   WebApiConfig.Register(config);

        }

       

        public int Priority
        {
            get
            {
                return 2;
            }
        }
    }
}
