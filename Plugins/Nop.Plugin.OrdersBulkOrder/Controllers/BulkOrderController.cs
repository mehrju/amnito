using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Payments;
using Nop.Web.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Nop.Plugin.Orders.BulkOrder.Controllers
{
    public class BulkOrderController : BasePublicController
    {

        private readonly IBulkOrderService _bulkOrderService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _StoreContext;
        private readonly IPaymentService _paymentService;
        private readonly IExtendedShipmentService _extendedShipmentService;

        public BulkOrderController(IWorkContext workContext
            , IBulkOrderService bulkOrderService
            , ICustomerService customerService
            , IPriceFormatter priceFormatter
            , ILocalizationService localizationService
            , IDateTimeHelper dateTimeHelper
            , IHostingEnvironment hostingEnvironment
            , IStoreContext StoreContext
            , IPaymentService paymentService
            , IExtendedShipmentService extendedShipmentService
            )
        {
            _StoreContext = StoreContext;
            _workContext = workContext;
            _bulkOrderService = bulkOrderService;
            _customerService = customerService;
            _priceFormatter = priceFormatter;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _hostingEnvironment = hostingEnvironment;
            _paymentService = paymentService;
            _extendedShipmentService = extendedShipmentService;
        }

        public IActionResult Index(string sec)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            //if (sec == "09139064053")
            //    return View("~/Plugins/Orders.BulkOrder/Views/Index_Sh.cshtml");
            if (_StoreContext.CurrentStore.Id == 3)
                return Content("این سرویس در دسترس نمی باشد");
            else //(_StoreContext.CurrentStore.Id == 5)
                return View("~/Plugins/Orders.BulkOrder/Views/Index_Sh.cshtml");
        }
        public IActionResult Index2(string sec, bool isPeyk)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            if (_StoreContext.CurrentStore.Id == 3)
                return Content("این سرویس در دسترس نمی باشد");
            else //(_StoreContext.CurrentStore.Id == 5)
            {
                BulkOrderModel _model = new BulkOrderModel();
                _model._isPeyk = isPeyk;
                _model.PhoneOrdermodel = null;
                return View("~/Plugins/Orders.BulkOrder/Views/Index_Sh2.cshtml", _model);
            }
        }
        public IActionResult Index_forPhoneOrder()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();
            if (_StoreContext.CurrentStore.Id == 3)
                return Content("این سرویس در دسترس نمی باشد");
            string str_PhoneOrdermodel = (string)TempData["PhoneOrdermodel"];
            phoneOrderRegisterOrderModel PhoneOrdermodel = Newtonsoft.Json.JsonConvert.DeserializeObject<phoneOrderRegisterOrderModel>(str_PhoneOrdermodel);
            BulkOrderModel _model = new BulkOrderModel();
            _model._isPeyk = false;
            _model.PhoneOrdermodel = PhoneOrdermodel;
            return View("~/Plugins/Orders.BulkOrder/Views/Index_Sh2.cshtml", _model);
        }
        public IActionResult ApIndex()
        {
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return Challenge();
            //if (_StoreContext.CurrentStore.Id == 3)
            //    return Content("این سرویس در دسترس نمی باشد");
            return View("~/Plugins/Orders.BulkOrder/Views/Index_Ap.cshtml");
        }
        [HttpPost]
        public IActionResult deleteBulkOrder(int bulkOrderId)
        {
            string msg = "";
            var Success = _bulkOrderService.DeleteBulkOrder(bulkOrderId, _workContext.CurrentCustomer.Id, out msg);
            return Json(new { Success, msg });
        }
        [HttpPost]
        public IActionResult SaveOrderList()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            var fileBinary = httpPostedFile.GetDownloadBits();

            var qqFileNameParameter = "qqfilename";
            string IsCodParameter = "IsCod";
            string discountCouponCodePrm = "discountCouponCode";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = Path.GetFileName(fileName);
            bool IsCod = bool.Parse(Request.Form[IsCodParameter].ToString());
            string discountCouponCode = Request.Form[discountCouponCodePrm].ToString();

            var contentType = httpPostedFile.ContentType;
            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            string[] validFileExtension = { ".xsl", ".xlsx" };
            if (validFileExtension.All(p => p != fileExtension))
                return Json(new
                {
                    success = false,
                    message = "فرمت فایل وارد شده نامعتبر می باشد"
                });
            var uniqFileName = _bulkOrderService.GetUniqueFileName(fileName);
            if (_bulkOrderService.ReadExcelFile(new MemoryStream(fileBinary), uniqFileName,
                _workContext.CurrentCustomer.Id, IsCod, discountCouponCode))
                return Json(new
                {
                    success = true,
                    message = "عملیات با موفقیت انجام شد"
                });

            return Json(new
            {
                success = false,
                message = "بروز مشکل در زمان خواندن فایل"
            });
        }


        [HttpPost]
        public IActionResult GetOrderList(DataSourceRequest command)
        {
            var DataCont = 0;
            var orders = _bulkOrderService.getBulkOrderList(out DataCont, command.Page - 1, command.PageSize,
                _workContext.CurrentCustomer.Id);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x => new
                {
                    x.Id,
                    OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                    OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.OrderStatusId,
                    PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.PaymentStatusId,
                    CustomerFullName = x.CustomerName,
                    CreateDate = _dateTimeHelper.ConvertToUserTime(x.CreateDate, DateTimeKind.Utc),
                    x.OrderId,
                    x.FileName,
                    x.OrderCount,
                    IsCod = x.IsCod ? "پس کرایه-COD" : "پیش کرایه"
                }),
                Total = DataCont
            };
            return Json(gridModel);
        }

        [HttpGet]
        public FileResult getExcelTamplate()
        {
            var filePath = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

            filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            var fileInfo = provider.GetFileInfo("OrderList_Template.xlsx");
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, "OrderList_Template.xlsx");
        }

        public FileResult getExcelTamplateCOD()
        {
            var filePath = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

            filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            var fileInfo = provider.GetFileInfo("OrderList_TemplateCOD.xlsx");
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, "OrderList_TemplateCOD.xlsx");
        }

        #region postex
        [HttpPost]
        public IActionResult SaveOrderList_Sh()
        {

            try
            {
                var httpPostedFile = Request.Form.Files.FirstOrDefault();
                if (httpPostedFile == null)
                    return Ok(new
                    {
                        success = false,
                        error = "No file uploaded",
                        downloadGuid = Guid.Empty
                    });
                var fileBinary = httpPostedFile.GetDownloadBits();

                var qqFileNameParameter = "qqfilename";
                string discountCouponCodePrm = "discountCouponCode";
                var fileName = httpPostedFile.FileName;
                if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                    fileName = Request.Form[qqFileNameParameter].ToString();
                //remove path (passed in IE)
                fileName = Path.GetFileName(fileName);

                int FileType = int.Parse(Request.Form["FileType"]);
                int ServiceSort = int.Parse(Request.Form["ServiceSort"]);
                bool HasAccessToPrinter = bool.Parse(Request.Form["HasAccessToPrinter"]);
                bool PrintLogo = bool.Parse(Request.Form["PrintLogo"]);
                bool SendSms = bool.Parse(Request.Form["SendSms"]);
                int ServiceId = int.Parse(Request.Form["ServiceId"]);
                string senderFirstName = Request.Form["SenderFirstName"];
                string senderLastName = Request.Form["SenderLastName"];
                string senderMobile = Request.Form["SenderMobile"];
                string senderState = Request.Form["SenderState"].ToString().Replace(" ", "_");
                string senderCity = Request.Form["SenderCity"].ToString().Replace(" ", "_");
                string senderPostCode = Request.Form["SenderPostCode"];
                string senderAddress = Request.Form["SenderAddress"];
                string senderEmail = Request.Form["SenderEmail"];
                string senderLat = Request.Form["SenderLat"];
                string senderLong = Request.Form["SenderLong"];


                string discountCouponCode = Request.Form[discountCouponCodePrm].ToString();


                var contentType = httpPostedFile.ContentType;
                var fileExtension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                string[] validFileExtension = { ".xlsx" };
                if (validFileExtension.All(p => p != fileExtension))
                    return Ok(new
                    {
                        success = false,
                        error = "فرمت فایل وارد شده نامعتبر می باشد"
                    });
                var uniqFileName = _bulkOrderService.GetUniqueFileName(fileName);
                BulkOrderModel bulkOrder = null;
                if (senderCity == "شهر_تهران")
                    senderCity = "منطقه_جنوب_شرق_(_17_پستی_)";
                if (_bulkOrderService.ReadExcelFile_PostKhone(new MemoryStream(fileBinary), uniqFileName,
                    _workContext.CurrentCustomer.Id, discountCouponCode, PrintLogo, SendSms, ServiceSort, FileType, HasAccessToPrinter, out bulkOrder,
                    senderFirstName, senderLastName, senderMobile, "", senderState, senderCity, senderPostCode, senderAddress, senderEmail, senderLat, senderLong, ServiceId))
                {
                    if (bulkOrder != null)
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

                            if (Errors.Any())
                            {
                                return Ok(new { success = false, error = "ثبت سفارش انجام شد ولی خطا زیر در بعضی از رکورد ها اتفاق افتاده و مانع از ثبت آنها شده" + "\r\n" + string.Join(",", Errors) });
                            }
                            else
                            {
                                bulkOrder.OrderStatusId = 20;
                                _bulkOrderService.UpdateBulkOrder(bulkOrder);
                                return Ok(new { success = true, error = "عملیات با موفقیت انجام شد" });
                            }
                        }
                        else
                        {
                            return Ok(new { success = false, error = "لطفا خطا های زیر را بررسی کنید" + "\r\n" + string.Join(",", Errors) });
                        }
                    }
                    return Ok(new
                    {
                        success = true,
                        error = "عملیات با موفقیت انجام شد"
                    });
                }
                return Ok(new
                {
                    success = false,
                    error = "خطا در زمان خواندن فایل، لطفا اطلاعات فایل اکسل را مجدد بررسی بفرمایید"
                });
            }
            catch (Exception ex)
            {
                plugin.Orders.ExtendedShipment.Services.common.Log("mybulkorder", ex.ToString());
                LogException(ex);
                return Ok(new
                {
                    success = false,
                    error = "خطا در زمان خواندن فایل، با پشتیبانی تماس بگیرید"
                });
            }
        }

        [HttpPost]
        public IActionResult GetOrderList_Sh(DataSourceRequest command)
        {
            var DataCont = 0;
            var orders = _bulkOrderService.getBulkOrderList(out DataCont, command.Page - 1, command.PageSize,
                _workContext.CurrentCustomer.Id);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x => new
                {
                    x.Id,
                    OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                    OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.OrderStatusId,
                    PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    x.PaymentStatusId,
                    CustomerFullName = x.CustomerName,
                    CreateDate = _dateTimeHelper.ConvertToUserTime(x.CreateDate, DateTimeKind.Utc),
                    x.OrderId,
                    x.FileName,
                    x.OrderCount,
                    _FileType = !x.FileType.HasValue ? "" : ((x.FileType == 1 ? "پست داخلی" : (x.FileType.Value == 2 ? "پست داخلی-پرداخت توسط گیرنده" : "پست بین المللی"))),
                    _ServiceSort = !x.ServiceSort.HasValue ? "" : (x.ServiceSort == 1 ? "ارزانترین" : "سریع ترین"),
                    _PrintLogo = !x.PrintLogo.HasValue ? "" : (x.PrintLogo.Value ? "بلی" : "خیر"),
                    _SendSms = !x.SendSms.HasValue ? "" : (x.SendSms.Value ? "بلی" : "خیر"),
                    _HasAccessToPrinter = (x.HasAccessToPrinter.HasValue && x.HasAccessToPrinter.Value ? "بلی" : "خیر")
                }),
                Total = DataCont
            };
            return Json(gridModel);
        }
        public FileResult getExcelTamplate_Pk()
        {
            var filePath = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

            filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            var fileInfo = provider.GetFileInfo("Pk_OrderList_Template.xlsx");
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, "Pk_OrderList_Template.xlsx");
        }
        public FileResult getExcelTamplateCOD_Pk()
        {
            var filePath = @"Plugins\Orders.BulkOrder\BulkOrderFile\";

            filePath = Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
            IFileProvider provider = new PhysicalFileProvider(filePath);
            var fileInfo = provider.GetFileInfo("Pk_OrderList_TemplateCOD.xlsx");
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.ms-excel";
            return File(readStream, mimeType, "Pk_OrderList_TemplateCOD.xlsx");
        }
        public IActionResult getServiceByFileType(int FileType, bool ispeyk = false)
        {
            var data = _bulkOrderService.getcategoryFileType(FileType, ispeyk);
            return Json(data);
        }
        public IActionResult getServiceInfo(int ServiceId)
        {
            var ServiceInfo = _extendedShipmentService.GetCategoryInfo(ServiceId);
            return Json(ServiceInfo);
        }

        public IActionResult _getApBulkOrderList(int pageIndex, int pageSize)
        {
            if (pageSize == 0)
                pageSize = 4;
            if (pageIndex == 0)
                pageIndex = 1;
            int DataCount = 0;
            var orders = _bulkOrderService.getBulkOrderList(out DataCount, pageIndex - 1, pageSize,
                _workContext.CurrentCustomer.Id)
            .Select(x => new BulkOrderItemMOdel()
            {
                Id = x.Id,
                OrderTotal = (int)x.OrderTotal,
                OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                OrderStatusId = x.OrderStatusId,
                PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                PaymentStatusId = x.PaymentStatusId,
                CustomerFullName = x.CustomerName,
                CreateDate = MiladyToShamsi(x.CreateDate, false),
                OrderId = x.OrderId,
                FileName = x.FileName,
                OrderCount = x.OrderCount,
                IsCod = x.IsCod ? "پرداخت در محل" : "پرداخت آنلاین",
                _FileType = (string.IsNullOrEmpty(x.CategoryName) ? (!x.FileType.HasValue ? "" : ((x.FileType == 1 ? "پست داخلی" : (x.FileType.Value == 2 ? "پست داخلی-پرداخت توسط گیرنده" : "پست بین المللی")))) : x.CategoryName),
                _ServiceSort = !x.ServiceSort.HasValue ? "" : ((x.ServiceSort == 1 || x.ServiceSort == 0) ? "ارزانترین" : "سریع ترین"),
                _PrintLogo = !x.PrintLogo.HasValue ? "" : (x.PrintLogo.Value ? "بلی" : "خیر"),
                _SendSms = !x.SendSms.HasValue ? "" : (x.SendSms.Value ? "بلی" : "خیر"),
                _HasAccessToPrinter = (x.HasAccessToPrinter.HasValue && x.HasAccessToPrinter.Value ? "دارم" : "ندارم"),
                count = DataCount

            }).ToList();
            return Json(orders);
        }
        private string MiladyToShamsi(DateTime dt, bool AddTime = false)
        {
            PersianCalendar pa = new PersianCalendar();
            string PaData = pa.GetYear(dt) + "/" + pa.GetMonth(dt).ToString("00") + "/" + pa.GetDayOfMonth(dt).ToString("00");
            if (AddTime)
                PaData += " " + dt.ToShortTimeString();
            return PaData;
        }
        #endregion
    }
    public class BulkOrderItemMOdel
    {
        public int Id { get; set; }
        public int OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public int OrderStatusId { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public string CustomerFullName { get; set; }
        public string CreateDate { get; set; }
        public int OrderId { get; set; }
        public string FileName { get; set; }
        public int OrderCount { get; set; }
        public string IsCod { get; set; }
        public string _FileType { get; set; }
        public string _ServiceSort { get; set; }
        public string _PrintLogo { get; set; }
        public string _SendSms { get; set; }
        public string _HasAccessToPrinter { get; set; }
        public int count { get; set; }

    }
}