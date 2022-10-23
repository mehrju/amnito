using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace NopMaster.Plugin.Payments.Wallet
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 2147483647;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute(
                "MonyBagOrderList"
                , "Admin/Order/OrderList",
                new {controller = "WalletAdmin", action = "OrderList", area = "admin"});
            routeBuilder.MapLocalizedRoute("MonyBagOrderList1", "Admin/Order/OrderList/",
                new {controller = "WalletAdmin", action = "OrderList"});
        }
    }
}