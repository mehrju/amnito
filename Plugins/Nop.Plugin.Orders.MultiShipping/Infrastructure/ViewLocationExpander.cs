using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Orders.MultiShipping.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {

            //if(context.ControllerName== "Home" && context.ViewName=="Index" && string.IsNullOrEmpty(context.AreaName))
            //{
            //    viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/NewCheckout/Index.cshtml" }.Concat(viewLocations);
            //}
            if (context.ViewName == "Components/OrderTotals/Default")
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/Components/OrderTotals/Default.cshtml" }.Concat(viewLocations);
            }
            if (context.ViewName == "TopicDetails")
            {
                var storeId = EngineContext.Current.Resolve<IStoreContext>().CurrentStore.Id;
                string TopicArea = (storeId == 5 ? "PostexNew" : "postbar");
                //if (context.ActionContext.HttpContext.Request.Query.ContainsKey("Ap"))
                //    viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/TopicDetails.cshtml" }.Concat(viewLocations);
                //else
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/NewCheckout/{TopicArea}/TopicDetails.cshtml" }.Concat(viewLocations);
            }
            if (context.ViewName == "BillingAddress" && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/Checkout/BillingAddress.cshtml" }.Concat(viewLocations);
            }
            if (context.ViewName == "ShippingAddress" && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/Checkout/ShippingAddress.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "Details" && string.IsNullOrEmpty(context.AreaName)
                && context.ControllerName == "Order")
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/Details.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_OrderDetails.Shipping")
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/_OrderDetails.Shipping.cshtml" }.Concat(viewLocations);
            }
            else if ((context.ViewName == "_Mus_OrderReviewData" || context.ViewName == "_Mus_CheckoutAttributes" || (context.ViewName == "Addresses")) && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.MultiShipping/Views/{{0}}.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName.Contains("Comments") && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { "/Plugins/Orders.MultiShipping/Views/{0}.cshtml" }.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
