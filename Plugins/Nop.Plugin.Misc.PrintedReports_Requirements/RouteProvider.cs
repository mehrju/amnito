using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //findAndRemoveRoute(routeBuilder.Routes, "ProductReviews");
            //findAndRemoveRoute(routeBuilder.Routes, "ProductDetails");
            //findAndRemoveRoute(routeBuilder.Routes, "ProductSearch");
            //findAndRemoveRoute(routeBuilder.Routes, "ShoppingCart");
            //findAndRemoveRoute(routeBuilder.Routes, "Search");
            //findAndRemoveRoute(routeBuilder.Routes, "ProductSearch");
            //findAndRemoveRoute(routeBuilder.Routes, "ProductSearchAutoComplete");
            //findAndRemoveRoute(routeBuilder.Routes, "ProductsByTag");



            //routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PrintedReports_Requirements",
            //   "SaleCartonwrapper",
            //   new { controller = "SaleCartonwrapper", action = "ProductDetail" });


            //var dataTokens = new RouteValueDictionary() {
            //    { "Namespaces",new[] { "Nop.Plugin.Orders.MultiShipping.Controllers" } }
            //};
            //routeBuilder.MapLocalizedRoute(
            // "_Sh_ConfirmAndPaySaleCarton"
            // , "_Sh_ConfirmAndPaySaleCarton"
            // , new { controller = "ShipitoCheckout", action = "ConfirmAndPaySaleCarton" }
            // , constraints: null
            // , dataTokens: dataTokens
            // );




            routeBuilder.MapLocalizedRoute("Nop.Plugin.Misc.PrintedReports_Requirements.ManageAvatarCustomer",
              "Admin/AvatarCustomer/{Id:regex(\\d*)}",
              new { controller = "ManageRouteProvider", action = "GetCustomer" }
          );
           
        }
        public int Priority
        {
            get
            {
                return 15;
            }
        }

        private bool findAndRemoveRoute(IList<IRouter> list, string routeName)
        {
            Route findedRoute = null;
            foreach (Route item in list)
            {
                if (string.Equals(item.Name, routeName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    findedRoute = item;
                    break;
                }
            }
            if (findedRoute != null)
            {
                list.Remove(findedRoute);
                return true;
            }
            return false;
        }
    }
}
