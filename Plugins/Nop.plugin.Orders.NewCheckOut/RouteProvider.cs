using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Payments.PayPalStandard
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
                { "Namespaces",new[] { "Nop.plugin.Orders.NewCheckOut.Controllers" } }
            }; 
            routeBuilder.MapLocalizedRoute(
                name:"Nop.plugin.Orders.NewCheckOut", template: "checkout2/",
                defaults: new { controller = "NewCheckout", action = "Index" },
                constraints: null,
                dataTokens: dataTokens
                );
            routeBuilder.MapRoute(
                name: "Nop.plugin.Orders.NewCheckOutOnePage", template: "onepagecheckout2/",
                defaults: new { controller = "NewCheckout", action = "OnePageCheckout" },
                constraints: null,
                dataTokens: dataTokens
                );
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return 2147483647; }
        }
    }
}
