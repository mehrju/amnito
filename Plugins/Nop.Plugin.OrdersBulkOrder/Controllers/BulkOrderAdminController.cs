using System;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Services.Payments;
using Nop.Services.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Controllers
{
    public class BulkOrderAdminController : BaseAdminController
    {
        private readonly IBulkOrderService _bulkOrderService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public BulkOrderAdminController(IWorkContext workContext
            , IBulkOrderService bulkOrderService
            , ICustomerService customerService
            , IPriceFormatter priceFormatter
            , ILocalizationService localizationService
            , IDateTimeHelper dateTimeHelper
            , IHostingEnvironment hostingEnvironment
            , IPaymentService paymentService
            , IOrderService orderService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _workContext = workContext;
            _bulkOrderService = bulkOrderService;
            _customerService = customerService;
            _priceFormatter = priceFormatter;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult BulkOrderAdminIndex()
        {
            if(!_workContext.CurrentCustomer.IsInCustomerRole("Administrators"))
                return AccessDeniedView();
            return View("~/Plugins/Orders.BulkOrder/Views/BulkOrderList.cshtml");
        }
        [HttpPost]
        public IActionResult getBulkOrdersList(DataSourceRequest command
            , BulkOrderModel model)
        {
            var dataCont = 0;
            var orders = _bulkOrderService.getBulkOrderList(out dataCont
                , command.Page - 1
                , command.PageSize
                , 0
                , model.CustomerName
                , model.CreateDateFrom
                , model.CreateDateTo
                , model.OrderStatusId
                , model.PaymentStatusId);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x => new
                {
                    Id = x.Id,
                    OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                    OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.OrderStatusId,
                    PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.PaymentStatusId,
                    CustomerFullName = x.CustomerName,
                    x.CreateDate,
                    OrderId = string.IsNullOrEmpty(x.OrderIds) ? x.OrderId.ToString() : x.OrderIds ,
                    x.FileName,
                    x.OrderCount,
                    IsCod = x.IsCod ? "پس کرایه-COD" : "پیش کرایه",
                    x.discountCouponCode
                }),
                Total = dataCont
            };
            return Json(gridModel);
        }
        [HttpGet]
        public FileResult getExcelFile(string FileName)
        {
            var filePath = @"Plugins\Orders.BulkOrder\BulkOrderFile\";
            filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            var fileInfo = provider.GetFileInfo(FileName);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, FileName);
        }

        public IActionResult changeStatus(int bulkorderId)
        {
            OrderStatus os = OrderStatus.Pending;
            var bulkOrder = _bulkOrderService.GetBulkOrder(bulkorderId);
            if (bulkOrder.IsInProcceing.HasValue && bulkOrder.IsInProcceing.Value)
            {
                return Json(new { success = true, message = "فایل مورد نظر در حال پردازش می باشد. لطفا صبر کنید" });
            }
            if (bulkOrder.OrderId > 0 || !string.IsNullOrEmpty(bulkOrder.OrderIds))
            {
                string OrderId = !string.IsNullOrEmpty(bulkOrder.OrderIds) ? bulkOrder.OrderIds : bulkOrder.OrderId.ToString();
                return Json(new { success = false, message = $"برای این رکورد سفارش با شماره {OrderId} ثبت گردیده" });
            }
            if (!(bulkOrder.FileType.HasValue && bulkOrder.FileType.Value > 0))
            {
                return Json(new { success = false, message = $"نوع فایل سفارش انبوه مورد نظر مشخص نشده" });
            }
            {
                if (os == OrderStatus.Pending)
                {
                    var Lst_placeOrderResult = _bulkOrderService.ProcessOrderList_PostKhones(bulkOrder);
                   
                    List<int> OrderIds = new List<int>();
                    List<string> Errors = new List<string>();
                    foreach (var item in Lst_placeOrderResult)
                    {
                        if (item.Success)
                        {
                            var postProcessPaymentRequest = new PostProcessPaymentRequest
                            {
                                Order = item.PlacedOrder
                            };
                            _paymentService.PostProcessPayment(postProcessPaymentRequest);
                            OrderIds.Add(postProcessPaymentRequest.Order.Id);
                        }
                        else
                        {
                            Errors.AddRange(item.Errors);
                        }
                    }
                    if (OrderIds.Any())
                    {
                        //if (OrderIds.Count() == 1)
                        bulkOrder.OrderStatusId = 20;
                        //else if (OrderIds.Count() > 1)
                        //    bulkOrder.OrderIds = string.Join(",", OrderIds);
                        _bulkOrderService.UpdateBulkOrder(bulkOrder);
                        if (Errors.Any())
                        {
                            return Json(new { success = false, message = "ثبت سفارش انجام شد ولی خطا زیر در بعضی از رکورد ها اتفاق افتاده و مانع از ثبت آنها شده" + "\r\n" + string.Join(",", Errors) });
                        }
                        else
                        {
                            return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "لطفا خطا های زیر را بررسی کنید" + "\r\n" + string.Join(",", Errors) });
                    }
                }
                else
                {
                    bulkOrder.OrderStatusId = 10;
                    _bulkOrderService.UpdateBulkOrder(bulkOrder);
                    if (bulkOrder.OrderId != 0)
                    {
                        var order = _orderService.GetOrderById(bulkOrder.OrderId);
                        order.OrderStatusId = 10;
                        _orderService.UpdateOrder(order);
                    }
                    return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
                }
            }
            //else
            //{
            //    if (os == OrderStatus.Pending)
            //    {
            //        var placeOrderResult = _bulkOrderService.ProcessOrderList(bulkOrder);
            //        if (placeOrderResult.Success)
            //        {
            //            var postProcessPaymentRequest = new PostProcessPaymentRequest
            //            {
            //                Order = placeOrderResult.PlacedOrder
            //            };
            //            _paymentService.PostProcessPayment(postProcessPaymentRequest);
            //            bulkOrder.OrderId = placeOrderResult.PlacedOrder.Id;
            //            _bulkOrderService.UpdateBulkOrder(bulkOrder);
            //            return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
            //        }
            //        return Json(new { success = false, message = "بروز خطا در زمان ثبت سفارشات" + "\r\n" + string.Join(",", placeOrderResult.Errors) });
            //    }
            //    else
            //    {
            //        bulkOrder.OrderStatusId = 10;
            //        _bulkOrderService.UpdateBulkOrder(bulkOrder);
            //        if (bulkOrder.OrderId != 0)
            //        {
            //            var order = _orderService.GetOrderById(bulkOrder.OrderId);
            //            order.OrderStatusId = 10;
            //            _orderService.UpdateOrder(order);
            //        }
            //        return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
            //    }
            //}
        }

        public IActionResult deleteBulkOrder(int bulkOrderId)
        {
            string msg = "";
            var success = _bulkOrderService.DeleteBulkOrder(bulkOrderId, 0, out msg);
            return Json(new { success, msg });
        }

    }
}