using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Plugin.Orders.MultiShipping.Models;

namespace Nop.Plugin.Orders.MultiShipping
{
    public class MultiShipingActionFilter : ActionFilterAttribute, IFilterProvider, IActionFilter
    {
        private readonly INewCheckout _newCheckout;
        public MultiShipingActionFilter(INewCheckout newCheckout)
        {
            _newCheckout = newCheckout;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpRequest = context.HttpContext.Request;
            string hostName = httpRequest.Host.Host;
            var Path = context.HttpContext.Request.Path;
            //var StoreAndSubMarket = _newCheckout.GetStoreAndSubMarket(hostName, Path);
            //var CurrentMarketInfo = context.HttpContext.Session.GetObject<SubMarketModel>("CurrentMarketInfo");
            //if(CurrentMarketInfo == null)
            //{
            //    CurrentMarketInfo = new SubMarketModel() { };
            //    context.HttpContext.Session.SetObject("CurrentMarketInfo", CurrentMarketInfo);
            //}

            var n = context.HttpContext.Request.GetDisplayUrl();//http://localhost:55390/
            var t = context.HttpContext.Request.Path;// "/"
            var p = context.RouteData;
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;
        }
        public void OnProvidersExecuted(FilterProviderContext context)
        {

        }

        public void OnProvidersExecuting(FilterProviderContext context)
        {

        }
    }
}
