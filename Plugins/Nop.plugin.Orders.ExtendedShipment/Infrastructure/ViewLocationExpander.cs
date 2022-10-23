using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.plugin.Orders.ExtendedShipment.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            //if (_storeContext.CurrentStore.Id != 3)
            //{
            //    return viewLocations;
            //}
            if (context.ViewName == "CustomerOrders" || context.ViewName == "ShipmentList" || context.ViewName == "ShipmentDetails" 
                || context.ViewName == "NewShipmentList" || context.ViewName == "Shipments")
            {
                if (context.ViewName == "ShipmentDetails" && string.IsNullOrEmpty(context.AreaName))
                {
                    return viewLocations;
                }
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/{{0}}.cshtml" }.Concat(viewLocations);
            }
            else if (context.ControllerName== "Shipments" && context.ViewName == "Index")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/Shipments/{{0}}.cshtml" }.Concat(viewLocations);
            } 
            else if (context.ControllerName== "gatewayShop" && context.ViewName == "Index")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/GetewayShop/{{0}}.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CustomerFnancialPlans")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CustomerFnancialPlans.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CustomerContractPlans")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CustomerContractPlans.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CustomerVehicles")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CustomerVehicles.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_MenuItem")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_MenuItem.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "Menu")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/Menu.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CreateOrUpdate" && context.AreaName == "Admin" && context.ControllerName == "Customer")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CreateOrUpdate.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "Login")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/Login.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "SmsNotifConfig")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/NotifConfiguration/SmsNotifConfig.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "PopupNotifConfig")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/NotifConfiguration/PopupNotifConfig.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_OrderDetails.Info")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_OrderDetails.Info.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "DefineAgentAmountRule")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/DefineAgentAmountRule.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "AssignAgentAmountRule")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/AssignAgentAmountRule.cshtml" }.Concat(viewLocations);
            }

            else if (context.ViewName == "_OrderStatistics")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_OrderStatistics.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_OrderDetails.Billing")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_OrderDetails.Billing.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CreateOrUpdate.RewardPoints")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CreateOrUpdate.RewardPoints.cshtml" }.Concat(viewLocations);
            }

            else if (context.ViewName == "Details" && context.ControllerName == "Order" &&
                string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/Details.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "Completed" && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/Completed.cshtml" }.Concat(viewLocations);
            }
            else if (context.ViewName == "_CreateOrUpdateAddress" && string.IsNullOrEmpty(context.AreaName))
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/_CreateOrUpdateAddress.cshtml" }.Concat(viewLocations);
            }
            else if (context.AreaName == "Admin" && context.ControllerName == "Order" && context.ViewName == "List")
            {
                viewLocations = new string[] { $"/Plugins/Orders.ExtendedShipment/Views/List.cshtml" }.Concat(viewLocations);
            }
            return viewLocations;

        }
    }
}
