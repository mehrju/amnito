using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Nop.Plugin.Payments.Mellat
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)             
        {
            routeBuilder.MapLocalizedRoute("NFCustomerInfo11", "Plugins/PaymentMellat/Pay/",
                new { controller = "NopFarsiPaymentMellat", action = "Pay" });
            

            routeBuilder.MapRoute("Plugin.Payments.Mellat.CallBack", "Plugins/PaymentMellat/CallBack/",
                new { controller = "NopFarsiPaymentMellat", action = "CallBack" });

            //routeBuilder.MapRoute("Plugin.Payments.Mellat.Pay", "Plugins/PaymentMellat/Pay/",
             //   new { controller = "PaymentMellat", action = "Pay" });

            routeBuilder.MapRoute("Plugin.Payments.Mellat.Pay1", "PaymentMellat/Error/{id?}",
                new { controller = "NopFarsiPaymentMellat", action = "Error" });
            routeBuilder.MapRoute("Plugin.Payments.Mellat.Pay2", "PaymentMellat/Error",
                new { controller = "NopFarsiPaymentMellat", action = "Error" });
            routeBuilder.MapRoute("Plugin.Payments.Mellat.Pay3", "Plugins/PaymentMellat/Error",
                new { controller = "NopFarsiPaymentMellat", action = "Error" });
        }

        public int Priority
        {
            get
            {
                return 10002;
            }
        }
    }
}
