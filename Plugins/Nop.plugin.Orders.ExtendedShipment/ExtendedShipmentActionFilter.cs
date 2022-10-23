using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;


namespace Nop.plugin.Orders.ExtendedShipment
{
    public class ExtendedShipmentActionFilter : ActionFilterAttribute, IFilterProvider, IActionFilter
    {
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly ISecurityService _securityService;
        private readonly IDbContext _dbContext;
        public ExtendedShipmentActionFilter(IProductService productService,
            IOrderService orderService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IExtendedShipmentService extendedShipmentService,
            IShipmentService shipmentService,
            ISecurityService securityService,
            IDbContext dbContext
        )
        {
            _dbContext = dbContext;
            _securityService = securityService;
            _workContext = workContext;
            _extendedShipmentService = extendedShipmentService;
            _shipmentService = shipmentService;
        }
        System.Diagnostics.Stopwatch watch;
        string actionName, controllerName;
        int CustomerId;
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(watch != null)
            {
                watch.Stop();
                LogActionCall(actionName, controllerName, _workContext.CurrentCustomer.Id, watch.ElapsedMilliseconds);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var p = context.RouteData;
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            actionName = actionDescriptor?.ActionName;
            controllerName = actionDescriptor?.ControllerName;
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;
            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (!_securityService.HasAccessToAction(context.HttpContext, actionName, controllerName))
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                context.Result = new JsonResult(new { HttpStatusCode.Unauthorized });
            }
           
            if (controllerName.ToLower() == "Order".ToLower()
                && actionName.Equals("ShipmentDetails", StringComparison.InvariantCultureIgnoreCase)
                && actionDescriptor.DisplayName.Contains("SetTrackingNumber"))
                try
                {
                    var model = (ShipmentModel)context.ActionArguments["model"];
                    var shipment = _shipmentService.GetShipmentById(model.Id);
                    string result = _extendedShipmentService.SetBarcodeIsUsed(shipment.TrackingNumber, shipment.Id);
                    if (result == "BarcodeReserved")
                    {
                        string msg = "بارکد " + "{0}" + " قبلا در سیستم ثبت شده";
                        msg = string.Format(msg, model.TrackingNumber);
                        context.Result = new JsonResult(new { msg = msg });
                        _extendedShipmentService.Log(msg, "");
                        return;
                    }
                    _extendedShipmentService.SendSmsForCustomerAndAdmin(shipment);

                }
                catch (Exception ex)
                {
                    _extendedShipmentService.Log("خطا در زمان ارسال پیامک اختصاص شماره رهگیری ", ex.Message +
                                                                                                 (ex.InnerException !=
                                                                                                  null
                                                                                                     ? ex.InnerException
                                                                                                         .Message
                                                                                                     : ""));
                }
            base.OnActionExecuting(context);
        }

        public void OnProvidersExecuting(FilterProviderContext context)
        {
            throw new NotImplementedException();
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
            throw new NotImplementedException();
        }
        private void LogActionCall(string ActionName,string Controller,int CustomerId,long time)
        {
            try
            {
                string query = $@"INSERT INTO dbo.Tb_ActionCallLog
                            (
	                            xActionName
	                            , xControllertName
	                            , xExcTime
	                            , xCustomerId
                                , xExcDate
                            )
                            VALUES
                            (	N'{ActionName}' -- xActionName - nvarchar(100)
	                            , N'{Controller}' -- xControllertName - nvarchar(100)
	                            , {time} -- xExcTime - int
	                            , {CustomerId} -- xCustomerId - int
                                , '{DateTime.Now.Year+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day+" "+DateTime.Now.ToString("HH:mm:ss")}'
	                        ) SELECT cast(SCOPE_IDENTITY() as int) AS Id";
                if (!string.IsNullOrEmpty(query))
                {
                    int id = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
            }
        }
        public int Order => int.MaxValue;
    }
}