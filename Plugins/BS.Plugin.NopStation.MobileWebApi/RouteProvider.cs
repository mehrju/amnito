
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
           // var config = GlobalConfiguration.Configuration;

            //config.EnableCors();
          //  config.MessageHandlers.Add(new CorsHandler());
            
            //routes.MapRoute(
            //    name: "DefaultWeb",
            //    template: "api/{controller}/{action}/{id?}"
                
            //);

           // routes.MapRoute("MobileWebApiConfigure",
           //     "Plugins/MobileWebApi/Generalsetting",
           //     new { controller = "MobileWebApiConfiguration", action = "Generalsetting" },
           //     new[] { "BS.Plugin.NopStation.MobileWebApi.Controllers" }
           //);
            routes.MapRoute("MobileWebApiConfigure.SliderImageAdd", "Plugins/MobileWebApi/SliderImageAdd",
            new
            {
                controller = "MobileWebApiConfiguration",
                action = "SliderImageAdd"
            });
         }


       

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
