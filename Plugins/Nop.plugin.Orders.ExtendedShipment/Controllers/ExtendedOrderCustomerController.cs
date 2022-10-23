using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ExtendedOrderCustomerController: BaseController
    {
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly OrderSettings _orderSettings;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IPdfService _pdfService;
        private readonly IOrderModelFactory _orderModelFactory;
        public ExtendedOrderCustomerController(
            IOrderModelFactory orderModelFactory,
            IExtendedShipmentService extendedShipmentService,
            OrderSettings orderSettings,
            IWorkContext workContext,
            IOrderService orderService,
            IPdfService pdfService
            )
        {
            _orderModelFactory = orderModelFactory;
            _pdfService = pdfService;
            _extendedShipmentService = extendedShipmentService;
            _orderSettings = orderSettings;
            _workContext = workContext;
            _orderService = orderService;
        }
        [HttpPost]
        public IActionResult GetOrderShipmentList(DataSourceRequest command,int orderId)
        {
            var DataCont = 0;
            var orderShipments = _extendedShipmentService.getOrderShipment(out DataCont, orderId, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = orderShipments.Select(x => new
                {
                    x.ShipmentId,
                    x.address,
                    x.DeliveryDateUtc,
                    x.FullName,
                    x.ProductName,
                    x.ShippedDateUtc,
                    x.TrackingNumber
                }),
                Total = DataCont
            };
            return Json(gridModel);
        }

        [HttpPost]
        [HttpGet]
        public IActionResult ChangeRoutOfCart()
        {
            if (_orderSettings.OnePageCheckoutEnabled && _workContext.OriginalCustomerIfImpersonated != null)
                return RedirectToRoute("CheckoutOnePage");
            return RedirectToAction("BillingAddress", "Checkout");
        }
        public IActionResult GetPdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }
        //[HttpsRequirement(SslRequirement.Yes)]
        //public virtual IActionResult Details(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return Challenge();

        //    var model = _orderModelFactory.PrepareOrderDetailsModel(order);
            
        //    //if(model.ShippingAddress == null)
        //    //{

        //    //}
        //    return View("/Plugins/Orders.ExtendedShipment/Views/Details.cshtml", model);
        //}
    }
}
