using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ShipmentTracking
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => -1;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute("LandingTrackingInfo", "rahgiry",
             new { controller = "ShipmentTracking", action = "ShipmentTrackingFrame" });
        }
    }
}
