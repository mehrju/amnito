using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using System;
using System.Linq;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class Ap_NewCheckoutController : BasePublicController
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
        public Ap_NewCheckoutController(
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
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Ap_NewChckOutOrder.cshtml", model);
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
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Ap_BulkCheckOutOrder.cshtml", model);
        }
        public IActionResult IndexTracking()
        {
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Tracking.cshtml");
        }

        public IActionResult ShowBillAndPayment([FromQuery] int[] orderIds)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ در قسمت صفحه در خواست امنیتو", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            var order = _orderService.GetOrderById(orderIds[0]);
            var billpayment = _newCheckout.GetFactorModel(orderIds.ToList());
            if (string.IsNullOrEmpty(order.PaymentMethodSystemName) || order.PaymentMethodSystemName != "Payments.CashOnDelivery")
            {
                int orderTotal = Convert.ToInt32(order.OrderTotal);
                string error = "";
                var _paymentrequest = _apService.CreatePaymentRequest(order.Id, out error);
                
                _newCheckout.Log("اطلاعات ارسالی جهت پرداخت در آپ", Newtonsoft.Json.JsonConvert.SerializeObject(_paymentrequest));
                ApPaymentModel model = new ApPaymentModel()
                {
                    paymentrequest = _paymentrequest,
                    OrderId = order.Id,
                    _billAndPaymentModel = billpayment
                };
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Ap_PaymentAndBill.cshtml", model);
            }
            else
            {
                ApPaymentModel model = new ApPaymentModel()
                {
                    IsCod = true,
                    OrderId = order.Id,
                    _billAndPaymentModel = billpayment
                };
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Ap_PaymentAndBill.cshtml", model);
            }
        }
        [HttpPost]
        public IActionResult ProccessPaymentResult(ApHostResponce model)
        {
            try
            {

                if (model == null)
                {
                    _newCheckout.Log("نتیجه پرداخت", "اطلاعات پرداخت نا معتبر می باشد");
                    return Json(new { message = "اطلاعات در یافتی نتیجه پرداخت نامعتبر می باشد", result = false });
                }
                var order = _orderService.GetOrderById(model.paymentId);
                var categoryInfo =_newCheckout.getCategoryInfo(order);
                bool IsCod = false;
                if(categoryInfo != null)
                {
                    IsCod = categoryInfo.IsCod;
                }
                if (!IsCod)
                {
                    string msg = "";
                    if (!_newCheckout.CanPayForOrder(order.Id, out msg))
                    {
                        return RedirectToRoute("CheckoutCompleted", new { orderId = model.paymentId });
                    }

                    string Str_model = JsonConvert.SerializeObject(model);
                    _newCheckout.Log("نتیجه پرداخت آپ", Str_model);

                    if (!string.IsNullOrEmpty(model.host_response))
                    {
                        model._host_response = JsonConvert.DeserializeObject<HostResponseModel>(model.host_response);

                        string error = "";
                        bool verfiyPayment = _apService.VarifyPayment(model, out error);
                        if (verfiyPayment)
                        {
                            
                            order.PaymentMethodSystemName = "NopFarsi.Payments.AsanPardakht";
                            _orderService.UpdateOrder(order);
                            _orderProcessingService.MarkOrderAsPaid(order);
                        }
                        string verifyResult = JsonConvert.SerializeObject(new { result = verfiyPayment, orderId = model.paymentId, message = error });
                        _newCheckout.Log("نتیجه verify کنترل درآپ", verifyResult);
                        return Json(new { result = verfiyPayment, orderId = model.paymentId, message = error });
                    }
                    else
                    {
                        _newCheckout.Log("نتیجه پرداخت آپ", Str_model);
                        return Json(new { message = "نتیجه پرداخت نامشخص، با پشتیبانی تماس بگیرید", result = false });
                    }
                }
                else
                {
                    order.PaymentMethodSystemName = "Payments.CashOnDelivery";
                    order.OrderStatusId = (int)OrderStatus.Processing;
                    _orderService.UpdateOrder(order);
                    return Json(new { result = true, orderId = model.paymentId, message = "" });
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
    public class ApPaymentModel
    {
        public BillAndPaymentModel _billAndPaymentModel { get; set; }
        public _paymentRequest paymentrequest { get; set; }
        public bool IsCod { get; set; }
        public int OrderId { get; set; }
    }
    public class paymentResult
    {
        public int paymentId { get; set; }
        public string message { get; set; }
        public string host_response { get; set; }
        public string host_response_sign { get; set; }
        public int status_code { get; set; }
        public string unique_tran_id { get; set; }
        public long payment_id { get; set; }
    }
}
