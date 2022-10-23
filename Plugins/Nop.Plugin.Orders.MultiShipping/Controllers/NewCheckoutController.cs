using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
//using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Orders.MultiShipping.Models.RSVP;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService;
//using static Nop.Web.Areas.Admin.Models.Customers.CustomerModel;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class NewCheckoutController : BasePublicController
    {
        #region fields
        private readonly ICountryService _countryService;
        private readonly INewCheckout _newCheckout;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IPermissionService _permissionService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        //private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IRewardPointService _rewardPointService;
        private readonly IPreOrderService _preOrderService;
        private readonly IStoreContext _storeContext;
        private readonly IReOrderService _reOrderService;
        private readonly IRepository<Tbl_RSVP_Webhook> _repositoryTbl_RSVP_Webhook;


        #endregion
        #region ctor
        public NewCheckoutController(
            IRepository<Tbl_RSVP_Webhook> repositoryTbl_RSVP_Webhook,
            ICountryService countryService
          //, IExtendedShipmentService extendedShipmentService
          , ICustomerActivityService customerActivityService
          , IOrderProcessingService orderProcessingService
          , ILocalizationService localizationService
          , IOrderService orderService
          , IPermissionService permissionService
          , INewCheckout newCheckout
          , IWorkContext workContext
          , IPaymentService paymentService
          , OrderSettings orderSettings
          , IWebHelper webHelper
          , IRewardPointService rewardPointService
          , IStoreContext storeContext,
            IReOrderService reOrderService,
            IPreOrderService preOrderService
          )
        {
            _repositoryTbl_RSVP_Webhook = repositoryTbl_RSVP_Webhook;
            _storeContext = storeContext;
            _reOrderService = reOrderService;
            _webHelper = webHelper;
            //_extendedShipmentService = extendedShipmentService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _permissionService = permissionService;
            _workContext = workContext;
            _countryService = countryService;
            _newCheckout = newCheckout;
            _orderSettings = orderSettings;
            _paymentService = paymentService;
            _rewardPointService = rewardPointService;
            _preOrderService = preOrderService;
        }
        #endregion
        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        public IActionResult Index(bool IsCod)
        {
            //if (_storeContext.CurrentStore.Id != 3)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("NewCheckout") + "?IsCod=" + IsCod.ToString() });
            if (IsCod && !_workContext.CurrentCustomer.IsInCustomerRole("COD"))
            {
                return RedirectToRoute("Nop.Plugin.Misc.PostbarDashboard.AddRequestCOD");
            }
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                IsCod = IsCod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Postbar/NewChckOutOrder.cshtml", model);
        }
        [HttpGet]
        public IActionResult getCountryList()
        {
            return Json(_newCheckout.getCountryList());
        }
        [HttpGet]
        public IActionResult getWeightItems()
        {
            return Json(_newCheckout.getWeightItem());
        }
        [HttpGet]
        public IActionResult getInsuranceItems()
        {
            return Json(_newCheckout.getInsuranceItems());
        }
        [HttpGet]
        public IActionResult getKartonItems()
        {
            return Json(_newCheckout.getKartonItems());
        }
        [HttpGet]
        public async Task<IActionResult> getServicesInfo(int senderCountry, int senderState, int receiverCountry, int receiverState,
            int weightItem, int AproximateValue)
        {
            return Json(await _newCheckout.GetServiceInfo(senderCountry, senderState, receiverCountry, receiverState,
                weightItem, AproximateValue, _workContext.CurrentCustomer.Id, IsFromUi: true));
        }
        [HttpGet]
        public IActionResult GetStatesByCountryId(int countryId)
        {
            return Json(_newCheckout.getStateByCountryId(countryId));
        }
        [HttpGet]
        public IActionResult FetchAddress(int countryId, int stateId, string searchtext)
        {
            List<CustomAddressModel> _Addresses = _newCheckout.FetchAddress(_workContext.CurrentCustomer.Id, countryId, stateId, searchtext).ToList();

            //var ThStateId = new List<int> { 4, 579, 580, 581, 582, 583, 584, 585 };
            //if (_Addresses.Any(p => p.CountryId == 1 && ThStateId.Contains(p.StateProvinceId.Value)))
            //{
            //    foreach (var item in _Addresses)
            //    {
            //        if (item.CountryId == 1 && ThStateId.Contains(item.StateProvinceId.Value))
            //        {
            //            item.StateProvinceId = 582;
            //            item.StateProvinceName = "شهر تهران";
            //        }
            //    }
            //}
            var myAddress = _Addresses.Select(p => new
            {
                id = p.Id.ToString(),
                p.text,
                p.FirstName,
                p.LastName,
                p.PhoneNumber,
                p.Company,
                p.Address1,
                p.ZipPostalCode,
                p.Email,
                p.Lat,
                p.Lon,
                p.StateProvinceId,
                p.StateProvinceName,
                CountryName = p.Country,
                p.CountryId,
            });
            return Json(new { results = myAddress });
        }
        [HttpPost]
        public IActionResult SaveNewCheckOutOrder(string JsonCheckoutModel)
        {
            if (!IsValidCustomer())
            {
                return Json(new { message = "کاربر گرامی ابتدا وارد سیستم شوید ،سپس اقدام به    ثبت سفارش کنید", success = false });
            }
            if (string.IsNullOrEmpty(JsonCheckoutModel))
            {
                _newCheckout.Log("متن ورودی خالی است", "");
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }
            List<NewCheckoutModel> CheckoutModel = new List<NewCheckoutModel>();
            try
            {
                CheckoutModel = JsonConvert.DeserializeObject<List<NewCheckoutModel>>(JsonCheckoutModel);
            }
            catch (Exception ex)
            {
                _newCheckout.Log(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }
            if (CheckoutModel == null || CheckoutModel.Count == 0)
            {
                return Json(new { message = "اطلاعات وارد شده نامعتبر می باشد", success = false });
            }

            if (_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.Id == 0 || _workContext.CurrentCustomer.IsGuest())
                return Json(new { message = "پس از ورود به حساب کاربری خود اقدام ثبت سفارش نمایید", success = false });

            var inputModel = new NewCheckout_Sp_Input()
            {
                JsonOrderList = JsonCheckoutModel,
                JsonData = JsonConvert.SerializeObject(new { CustommerId = _workContext.CurrentCustomer.Id, CustommerIp = _webHelper.GetCurrentIpAddress(), StoreId = _storeContext.CurrentStore.Id, SourceId = 1 })
            };
            try
            {
                var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Normal, 0, false, false);
                if (ret.orderId > 0)
                {
                    _reOrderService.InsertOrderJson(ret.orderId, JsonCheckoutModel);
                    return Json(new { success = true, message = "ثبت سفارش با موفقیت انجام شد", orderIds = ret.orderId, });
                }
                else
                {
                    return Json(new { success = false, message = ret.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { message = "بروز اشکال در زمان ثبت سفارش.لطفا مجدد سعی در ثبت سفارش کنید", success = false });
            }
            //**************************************
            List<string> str_msg = new List<string>();
            foreach (var item in CheckoutModel)
            {
                string Message = "";
                item.IsValid(out Message);
                if (!string.IsNullOrEmpty(Message))
                {
                    str_msg.Add($@"سفارش شماره {item.TempId} : \r\n" + Message);
                }
            }
            if (str_msg.Any())
                return Json(new { message = string.Join("\r\n", str_msg), success = false });
            if (CheckoutModel.Any(p => p.billingAddressModel.SumAddressValue() != CheckoutModel[0].billingAddressModel.SumAddressValue()))
                return Json(new { message = "در حال حاضر امکان ثبت سفارش از مبدا های متفاوت دریک سفارش فعال نمی باشد", success = false });
            List<int> orderIds = new List<int>();
            string outWarinig = "";
            List<int> successClientOrderIds = new List<int>();

            if (CheckoutModel.Any(p => p.billingAddressModel.SumAddressValue() != CheckoutModel[0].billingAddressModel.SumAddressValue()))
            {
                List<string> BliingAddresses = CheckoutModel.GroupBy(p => p.billingAddressModel.SumAddressValue()).Select(g => g.First().billingAddressModel.SumAddressValue()).ToList();
                foreach (var address in BliingAddresses)
                {
                    var sameOrders = CheckoutModel.Where(p => p.billingAddressModel.SumAddressValue() == address).ToList();
                    var placeOrderResult = _newCheckout.ProccessOrder(sameOrders, _workContext.CurrentCustomer.Id);
                    if (placeOrderResult.Success)
                    {
                        var orderId = placeOrderResult.PlacedOrder.Id;
                        orderIds.Add(orderId);
                        successClientOrderIds.AddRange(sameOrders.Select(p => p.TempId).ToArray());
                    }
                    else
                    {
                        outWarinig += string.Join("\r\n", placeOrderResult.Errors).Replace("erroCode:940", "").Replace("erroCode:930", "") + "\r\n";
                    }
                }
                return Json(new { success = (string.IsNullOrEmpty(outWarinig) ? true : false), message = outWarinig, orderIds, successClientOrderIds });
            }

            var _placeOrderResult = _newCheckout.ProccessOrder(CheckoutModel, _workContext.CurrentCustomer.Id);

            if (_placeOrderResult.Success)
            {
                var orderId = _placeOrderResult.PlacedOrder.Id;
                successClientOrderIds.Add(CheckoutModel[0].TempId);
                _reOrderService.InsertOrderJson(_placeOrderResult.PlacedOrder.Id, JsonCheckoutModel);
                return Json(new { success = true, message = "ثبت سفارش با موفقیت انجام شد", orderIds = orderId, });
            }

            string warning = string.Join("\r\n", _placeOrderResult.Errors);
            warning.Replace("erroCode:940", "").Replace("erroCode:930", "");
            return Json(new { success = false, message = "توجـــــــه" + "\r\n" + warning });


        }
        public IActionResult ShowBillAndPayment([FromQuery] int[] orderIds)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login");
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Postbar/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(orderIds.ToList()));
        }
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("BillAndPayment") + "?orderIds[0]=" + id });

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return Json(new { success = false, message = "اطلاعات مربوط به سفارش شما یافت نشد" });
            if (order.OrderStatus == OrderStatus.Cancelled)
                return Json(new { success = false, message = "انصراف از سفارش با موفقیت انجام شده. مجددا سعی نکنید" });
            try
            {
                _orderProcessingService.CancelOrder(order, true);
                LogEditOrder(order.Id);
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    return Json(new { success = true, message = "انصراف از سفارش با موفقیت انجام شد" });
                }
                return Json(new { success = false, message = "در حال حاضر امکان انصراف از سفارش وجود ندارد. با پشتیبانی سامانه تماس بگیرید" });
            }
            catch (Exception exc)
            {
                //error
                _newCheckout.InsertOrderNote("خطا در زمان انصراف از سفارش" + "\r\n"
                    + exc.Message + (exc.InnerException != null ? "-->" + exc.InnerException.Message : ""), id);
                return Json(new { success = false, message = "عملیات انصراف از شسفارش شما با مشکل مواجه شد. با پشتیبانی سامانه تماس بگیرید" });
            }
        }
        protected void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder", _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
        }
        [HttpPost]
        public IActionResult IsvalidService(int serviceId)
        {
            return Json(new { success = _newCheckout.IsValidServiceForCustomer(serviceId) });
        }
        [HttpPost]
        public IActionResult ConfirmAndPay(int orderId, string paymentmethod, bool UseRewardPoints)
        {
            TempData["PaymentMsg"] = null;
            var order = _orderService.GetOrderById(orderId);
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                TempData["PaymentMsg"] = "سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                return Redirect(Url.RouteUrl("BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=1");
            }

            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
            int rewardPointsBalance =
                           _rewardPointService.GetRewardPointsBalance(order.CustomerId, order.StoreId);

            bool isCod = order.IsOrderCod();
            if (!isCod)
            {
                if (UseRewardPoints)
                {
                    if (order.OrderTotal > rewardPointsBalance)
                    {
                        TempData["PaymentMsg"] = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(order.OrderTotal).ToString("N0")
                                                                                         + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
                        return Redirect(Url.RouteUrl("BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=2");
                    }
                    else
                    {
                        // TODO : reward point
                        _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -Convert.ToInt32(order.OrderTotal), order.StoreId,
                       string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
                       order, order.OrderTotal);
                        order.OrderTotal = 0;
                        order.PaymentMethodSystemName = null;
                        _orderProcessingService.MarkOrderAsPaid(order);
                        return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });

                    }
                }
                if (string.IsNullOrEmpty(paymentmethod))
                {
                    TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
                    return Redirect(Url.RouteUrl("BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=3");

                }
                order.PaymentMethodSystemName = paymentmethod;
                _orderService.UpdateOrder(order);
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };
                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content("Redirected");
                }
            }
            else
            {
                order.OrderStatusId = (int)OrderStatus.Processing;
                _orderService.UpdateOrder(order);
            }
            return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
        }

        public IActionResult ChargeWalletIndex()
        {
            if (!IsValidCustomer())
            {
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_ChargeWallet") });
            }
            var model = _newCheckout.getPaymentMethodForChargeWallet();
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Postbar/ChargeWallet.cshtml", model);
        }
        [HttpPost]
        public IActionResult ChargeWallet(string paymentmethod, int amount)
        {
            TempData["PaymentMsg"] = null;
            if (!IsValidCustomer())
            {
                TempData["PaymentMsg"] = "ابتدا وارد سیستم شوید سپس اقدام به پرداخت کنید";
                return Redirect(Url.RouteUrl("_ChargeWallet"));// TODO: باید تغییر کنید
            }
            if (string.IsNullOrEmpty(paymentmethod))
            {
                TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
                return Redirect(Url.RouteUrl("_ChargeWallet"));// TODO: باید تغییر کنید

            }
            if (amount <= 0)
            {
                TempData["PaymentMsg"] = "مبلغ وارد شده نامعتبر می باشد";
                return Redirect(Url.RouteUrl("_ChargeWallet"));// TODO: باید تغییر کنید
            }
            var checkoutReult = _newCheckout.ProccessWalletOrder(amount, paymentmethod, _workContext.CurrentCustomer.Id);
            if (!checkoutReult.Success)
            {
                TempData["PaymentMsg"] = string.Join("\r\n", checkoutReult.Errors);
                return Redirect(Url.RouteUrl("_ChargeWallet"));// TODO: باید تغییر کنید
            }
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = checkoutReult.PlacedOrder
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("در حال انتقال به درگاه");
            }

            return RedirectToRoute("CheckoutCompleted", new { orderId = checkoutReult.PlacedOrder.Id });
        }


        [HttpGet]
        public IActionResult DetectRSVP()
        {
            var detect = _repositoryTbl_RSVP_Webhook.Table.Where(p => p.Mobile.Contains(_workContext.CurrentCustomer.Username)).FirstOrDefault();
            if (detect != null)
            {
                return Json(new { success = true, data = "1" });
            }
            else
            {
                return Json(new { success = true, data = "0" });
            }

        }


        [HttpPost]
        public IActionResult GetLatLong(int? CountryId, int? StatePrivenceId)
        {
            var result = _newCheckout.GetLatLong(CountryId, StatePrivenceId);
            return Json(new { Lat = result.Lat, Lon = result.Lon });
        }

        
        public IActionResult PreOrderInfo([FromQuery] int PreOrderId)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login");
            var model = _preOrderService.PreOrderCheckout(PreOrderId);
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/PreOrderInfo.cshtml", model);
        }
    }

}
