using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Localization;
using System;

namespace Nop.Plugin.Orders.BulkOrder
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            var dataTokens = new RouteValueDictionary() {
                { "Namespaces",new[] { "Nop.Plugin.Orders.BulkOrder.Controllers" } }
            };

            routeBuilder.MapLocalizedRoute(
                "BulkOrder"
                , "BulkOrder"
                , new { controller = "BulkOrder", action = "Index" }
                , constraints: null
                , dataTokens: dataTokens
                );
            routeBuilder.MapLocalizedRoute(
                "BulkOrder2"
                , "BulkOrder2"
                , new { controller = "BulkOrder", action = "Index2" }
                , constraints: null
                , dataTokens: dataTokens
                );
            routeBuilder.MapLocalizedRoute(
               "ApBulkOrder"
               , "ApBulkOrder"
               , new { controller = "BulkOrder", action = "ApIndex" }
               , constraints: null
               , dataTokens: dataTokens
               );
            //routeBuilder.MapLocalizedRoute(
            //    "BulkOrderList"
            //    , "BulkOrderList"
            //    , new { controller = "BulkOrder", action = "BulkOrderAdminIndex",area = "admin" }
            //    , constraints: null
            //    , dataTokens: dataTokens
            //);
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => Int32.MaxValue;

    }
}
