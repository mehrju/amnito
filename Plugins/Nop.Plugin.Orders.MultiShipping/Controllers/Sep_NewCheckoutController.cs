using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class Sep_NewCheckoutController : BasePublicController
    {
        #region fileds
        private readonly INewCheckout _newCheckout;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IApService _apService;
        private readonly IOrderProcessingService _orderProcessingService;
        #endregion

        #region ctor
        public Sep_NewCheckoutController(
           IOrderService orderService
          , INewCheckout newCheckout
          , IWorkContext workContext
          , IStoreContext storeContext
          , IApService apService
          , IOrderProcessingService orderProcessingService
          )
        {
            _apService = apService;
            _storeContext = storeContext;
            _orderService = orderService;
            _workContext = workContext;
            _newCheckout = newCheckout;
            _orderProcessingService = orderProcessingService;
        }
        #endregion

        #region ActionsController
        public IActionResult Index(string pa)
        {
            //درخواست اطلاعات کاربر
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal))
            };

            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/sep_NewChckOutOrder.cshtml", model);
        }
        public IActionResult BulkIndex()
        {
            //درخواست اطلاعات کاربر
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
            };

            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/sep_BulkCheckOutOrder.cshtml", model);
        }
        public IActionResult IndexTracking()
        {
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/Tracking.cshtml");
        }

        public IActionResult ShowBillAndPayment([FromQuery] int orderId, [FromQuery]bool IsPaymentSuccess = true)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر 724 در قسمت صفحه در خواست امنیتو", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            if (!IsPaymentSuccess)
            {
                SepPaymentModel model = new SepPaymentModel();
                if (orderId != 0)
                {
                    var _order = _orderService.GetOrderById(orderId);
                   // model.paymentrequest = $@"seppay://{12152457}/{_order.Id}/0|{Convert.ToInt32(_order.OrderTotal)}";
                    model.paymentrequest = $@"seppay://{396}/{_order.Id}/0|{Convert.ToInt32(_order.OrderTotal)}";
                    model._billAndPaymentModel = _newCheckout.GetFactorModel(new List<int>(orderId));
                    model.OrderId = orderId;
                }
                model.IsSuccess = false;
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/sep_PaymentAndBill.cshtml", model);
            }
            var order = _orderService.GetOrderById(orderId);
            var billpayment = _newCheckout.GetFactorModel(new List<int>() { orderId });
            if (string.IsNullOrEmpty(order.PaymentMethodSystemName) || order.PaymentMethodSystemName != "Payments.CashOnDelivery")
            {
                int orderTotal = Convert.ToInt32(order.OrderTotal);
                string error = "";
                //var _paymentrequest = $@"seppay://{12152457}/{order.Id}/0|{Convert.ToInt32(order.OrderTotal)}";
                var _paymentrequest = $@"seppay://{396}/{order.Id}/0|{Convert.ToInt32(order.OrderTotal)}";

                _newCheckout.Log("اطلاعات ارسالی جهت پرداخت در 724", _paymentrequest);
                SepPaymentModel model = new SepPaymentModel()
                {
                    paymentrequest = _paymentrequest,
                    OrderId = order.Id,
                    _billAndPaymentModel = billpayment,
                    IsSuccess = true
                };
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/sep_PaymentAndBill.cshtml", model);
            }
            else
            {
                SepPaymentModel model = new SepPaymentModel()
                {
                    IsCod = true,
                    OrderId = order.Id,
                    _billAndPaymentModel = billpayment,
                    IsSuccess = true
                };
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/sep_PaymentAndBill.cshtml", model);
            }
        }
        [Route("api/App/SepPaymentCallBack")]
        [HttpGet]
        public IActionResult SepPaymentCallBack()
        {
            string _res_num = HttpContext.Request.Query["res_num"].ToString();
            string _msisdn = HttpContext.Request.Query["msisdn"].ToString();
            string _ref_num = HttpContext.Request.Query["ref_num"].ToString();
            string _trace_num = HttpContext.Request.Query["trace_num"].ToString();
            string _amount = HttpContext.Request.Query["amount"].ToString();
            if (string.IsNullOrEmpty(_res_num))
            {
                common.Log("اطلاعات بازگشتی از پرداخت 724 نام معتبر میباشد", "");
                return RedirectToRoute("SepBillAndPayment", new { orderId = 0, IsPaymentSuccess = false });
            }
            try
            {
                var order = _orderService.GetOrderById(Int32.Parse(_res_num));
                if (order == null)
                {
                    common.Log("شماره سفارش ارسال شده در برگشت پرداخت 724 نام معتبر می باشد", "");
                    return RedirectToRoute("SepBillAndPayment", new { orderId = 0, IsPaymentSuccess = false });
                }
                RequestData _requestData = new RequestData()
                {
                    res_num = Int32.Parse(_res_num),
                    msisdn = _msisdn,
                    ref_num = _ref_num,
                    trace_num = _trace_num,
                    amount = Int32.Parse(_amount),
                    ShouldJson = false
                };
                string result = "";
                var data = new { mobile_no = _requestData.msisdn, ref_no = _requestData.ref_num.Replace(" ","+") };
                string JsonData = JsonConvert.SerializeObject(data).Trim();
                string url = "https://api.seppay.ir/1/verify";
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = JsonData;
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();
                        }
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            SepResponseCode respData = JsonConvert.DeserializeObject<SepResponseCode>(result);
                            _newCheckout.Log("پاسخ 724 در پرداخت سفارش" + _requestData.res_num.ToString(), result);
                            if (respData == null)
                            {
                                _newCheckout.InsertOrderNote("تراکنش پرداخت 724 با موفقیت پرداخت نشد", _requestData.res_num);
                                return RedirectToRoute("SepBillAndPayment", new { orderId = _requestData.res_num, IsPaymentSuccess = false });
                            }
                            if (string.IsNullOrEmpty(respData.ResponseCode) || respData.ResponseCode != "1")
                            {
                                _newCheckout.InsertOrderNote("تراکنش پرداخت 724 با موفقیت پرداخت نشد", _requestData.res_num);
                                return RedirectToRoute("SepBillAndPayment", new { orderId = _requestData.res_num, IsPaymentSuccess = false });
                            }
                            return ProccessPaymentResult(_requestData);
                        }
                    }
                }
                return RedirectToRoute("SepBillAndPayment", new { orderId = _requestData.res_num, IsPaymentSuccess = false });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return RedirectToRoute("SepBillAndPayment", new { orderId = 0, IsPaymentSuccess = false });
            }
        }
        public IActionResult ProccessPaymentResult(RequestData model)
        {
            try
            {
                var order = _orderService.GetOrderById(model.res_num);
                var categoryInfo = _newCheckout.getCategoryInfo(order);
                bool IsCod = false;
                if (categoryInfo != null)
                {
                    IsCod = categoryInfo.IsCod;
                }
                if (!IsCod)
                {
                    string msg = "";
                    if (!_newCheckout.CanPayForOrder(order.Id, out msg))
                    {
                        if (!model.ShouldJson)
                            return RedirectToRoute("SepCheckoutCompleted", new { orderId = model.res_num });
                        else
                            return Json(new { result = true });
                    }
                    string Str_model = JsonConvert.SerializeObject(model);
                    _newCheckout.Log("نتیجه پرداخت 724", Str_model);

                    order.PaymentMethodSystemName = "NopFarsi.Payments.SepShaparak";
                    order.AuthorizationTransactionId = model.trace_num;
                    _orderService.UpdateOrder(order);
                    _orderProcessingService.MarkOrderAsPaid(order);
                    if (!model.ShouldJson)
                        return RedirectToRoute("SepCheckoutCompleted", new { orderId = model.res_num });
                    else
                        return Json(new { result =true });
                }
                else
                {
                    order.PaymentMethodSystemName = "Payments.CashOnDelivery";
                    order.OrderStatusId = (int)OrderStatus.Processing;
                    _orderService.UpdateOrder(order);
                    if (!model.ShouldJson)
                        return RedirectToRoute("SepCheckoutCompleted", new { orderId = model.res_num });
                    else
                        return Json(new { result = true });
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { message = "خطا در زمان پردازش نتیجه پرداخت، با پشتیبانی تماس بگیرید", result = false });
            }
        }
        #endregion
    }
    public class SepPaymentModel
    {
        public BillAndPaymentModel _billAndPaymentModel { get; set; }
        public string paymentrequest { get; set; }
        public bool IsCod { get; set; }
        public int OrderId { get; set; }
        public bool IsSuccess { get; set; }
    }
    public class RequestData
    {
        /// <summary>
        /// شناسه فاکتور و یا رزرو پرداخت
        /// </summary>
        public int res_num { get; set; }
        /// <summary>
        /// شماره موبایل کاربر
        /// </summary>
        public string msisdn { get; set; }
        /// <summary>
        /// شماره ارجاع پرداخت
        /// </summary>
        public string ref_num { get; set; }
        /// <summary>
        /// شماره پیگیری پرداخت
        /// </summary>
        public string trace_num { get; set; }
        /// <summary>
        /// مبلغ پرداخت شده
        /// </summary>
        public int amount { get; set; }
        public bool IsCod { get; set; }
        public bool ShouldJson { get; set; }
    }
    public class SepResponseCode
    {
        public string ResponseCode { get; set; }
    }

}
