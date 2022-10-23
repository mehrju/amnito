using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Directory;
using Nop.Services.Helpers;
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

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class Ptx_CheckoutController : BasePublicController
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
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<ProductAttributeValue> _repositoryTbl_ProductAttributeValue;
        private readonly IReOrderService _reOrderService;
        #endregion
        #region ctor
        public Ptx_CheckoutController(ICountryService countryService
        //  , IExtendedShipmentService extendedShipmentService
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
          , IStoreContext storeContext
          , IDateTimeHelper dateTimeHelper
            , IRepository<ProductAttributeValue> repositoryTbl_ProductAttributeValue,
            IReOrderService reOrderService
          )
        {
            _storeContext = storeContext;
            _webHelper = webHelper;
            // _extendedShipmentService = extendedShipmentService;
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
            _dateTimeHelper = dateTimeHelper;
            _repositoryTbl_ProductAttributeValue = repositoryTbl_ProductAttributeValue;
            _reOrderService = reOrderService;
        }
        #endregion

        #region MyRegion

        [HttpGet]
        public IActionResult getUbbarTruckType()
        {
            return Json(_newCheckout.getUbbarTruckType());
        }
        [HttpGet]
        public IActionResult getUbbarVechileOPtion(string TruckType)
        {
            return Json(_newCheckout.getUbbarVechileOption(TruckType));
        }
        #endregion
        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        public IActionResult Index(string pa, bool isCod, bool? isSafeBuy)
        {
            if (!IsValidCustomer())
            {
                string _returnUrl = Url.RouteUrl("_Sh_Checkout") + "?isCod=" + isCod.ToString() + $"&isSafeBuy={isSafeBuy.ToString()}" + (string.IsNullOrEmpty(pa) ? "" : $"&pa={pa}");
                return RedirectToRoute("Login", new { returnUrl = _returnUrl });
            }
            if (isCod && !(_workContext.CurrentCustomer.IsInCustomerRole("COD") || _workContext.CurrentCustomer.IsInCustomerRole("onlineGateway")))
            {
                return RedirectToRoute("Nop.Plugin.Misc.PostbarDashboard.AddRequestCOD");
            }
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD") || _workContext.CurrentCustomer.IsInCustomerRole("onlineGateway"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal)),
                IsCod = isCod,
                IsSafeBuy = isSafeBuy ?? false
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/ptx_NewChckOutOrder.cshtml", model);
        }

        public IActionResult SekeIndex(string pa, bool isCod)
        {

            if (!IsValidCustomer())
            {
                string _returnUrl = Url.RouteUrl("_Sh_Checkout") + "?isCod=" + isCod.ToString() + (string.IsNullOrEmpty(pa) ? "" : $"&pa={pa}");
                return RedirectToRoute("Login", new { returnUrl = _returnUrl });
            }
            if (isCod && !(_workContext.CurrentCustomer.IsInCustomerRole("COD") || _workContext.CurrentCustomer.IsInCustomerRole("onlineGateway")))
            {
                return RedirectToRoute("Nop.Plugin.Misc.PostbarDashboard.AddRequestCOD");
            }
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD") || _workContext.CurrentCustomer.IsInCustomerRole("onlineGateway"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal)),
                IsCod = isCod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/ptx_NewChckOutOrder.cshtml", model);
        }

        public IActionResult Bu_Index(string pa, bool isCod)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_Sh_Checkout") });
            if (isCod && !_workContext.CurrentCustomer.IsInCustomerRole("COD"))
            {
                return RedirectToRoute("Nop.Plugin.Misc.PostbarDashboard.AddRequestCOD");
            }
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                postArea = string.IsNullOrEmpty(pa) ? PostArea.Internal : (pa.ToLower() == "f" ? PostArea.Foreign : (pa.ToLower() == "h" ? PostArea.Heavy : PostArea.Internal)),
                IsCod = isCod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/NewChckOutOrder.cshtml", model);
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
        public IActionResult getForeginCOuntry()
        {
            return Json(_newCheckout.getForginCountry());
        }
        [HttpPost]
        public async Task<ActionResult> getServicesInfo(string _model)
        {
            try
            {
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<getServiceInfoModel>(_model);
                return Json(await _newCheckout.GetServiceInfo(model.senderCountry, model.senderState, model.receiverCountry, model.receiverState,
                    model.weightItem, model.AproximateValue, _workContext.CurrentCustomer.Id, model.height, model.length, model.width
                    , dispach_date: (string.IsNullOrEmpty(model.dispatch_date) ? (DateTime?)null : Convert.ToDateTime(model.dispatch_date)),
                    PackingOption: model.UbbarPackingLoad, vechileType: model.UbbraTruckType, VechileOption: model.VechileOptions, content: model.Content
                    , receiver_ForeginCountry: model.receiver_ForeginCountry, receiver_ForeginCountryNameEn: model.receiver_ForeginCountryNameEn, consType: model.boxType, IsCod: model.IsCod, IsFromAp: model.IsFromAp
                    , SenderAddress: model.SenderAddress, ReciverAddress: model.ReciverAddress, IsFromUi: true, IsFromSep: model.IsFromSep));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
        [HttpGet]
        public IActionResult GetStatesByCountryId(int countryId)
        {
            var stateList = _newCheckout.getStateByCountryId(countryId);
            //if (countryId == 1)
            //{
            //    List<string> ThPostarea = new List<string>() { "4", "579", "580", "581", "582", "583", "584", "585" };
            //    var NewStateList = stateList.Where(p => !ThPostarea.Contains(p.Value)).ToList();
            //    NewStateList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "579", Text = "شهر تهران" });
            //    return Json(NewStateList.OrderBy(p => p.Text).ToList());
            //}
            return Json(stateList);
        }
        [HttpGet]
        public IActionResult GetUbbarStatesByCountryId(int countryId)
        {
            return Json(_newCheckout.getUbbarStateByCountryId(countryId));
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
                p.CountryId
            });
            return Json(new { results = myAddress });
        }
        [HttpPost]
        public IActionResult SaveNewCheckOutOrder(string JsonCheckoutModel)
        {
            if (!IsValidCustomer())
            {
                return Json(new { message = "کاربر گرامی ابتدا وارد سیستم شوید ،سپس اقدام به ثبت سفارش کنید", success = false });
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
            {
                return Json(new { message = "در حال حاضر امکان ثبت سفارش از مبدا های متفاوت دریک سفارش فعال نمی باشد", success = false });
            }

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
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(orderIds.ToList()));
        }
        public IActionResult _ShowBillAndPayment([FromQuery] int orderId)
        {
            if (!IsValidCustomer())
                return RedirectToRoute("Login");
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(new List<int>() { orderId }));
        }
        public IActionResult _ShowSafeBuyBillAndPayment([FromQuery] int orderId)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PaymentAndBill.cshtml", _newCheckout.GetFactorModel(new List<int>() { orderId },true));
        }

        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + id });

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
        public IActionResult ConfirmAndPay(int orderId, string paymentmethod, bool UseRewardPoints, bool isForeign = false)
        {
            TempData["PaymentMsg"] = null;
            var order = _orderService.GetOrderById(orderId);
            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                TempData["PaymentMsg"] = "سفارش شما کنسل شده و امکان پرداخت برای آن وجود ندارد";
                return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=1");
            }

            if (order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid)
                return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
            int rewardPointsBalance =
                           _rewardPointService.GetRewardPointsBalance(order.CustomerId, order.StoreId);

            bool isCod = order.IsOrderCod();
            if (!isCod)
            {
                string msg = "";
                if (!_newCheckout.CanPayForOrder(order.Id, out msg))
                {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
                }
                AddtionalDataForginRequest(order);
                if (UseRewardPoints)
                {
                    if (order.OrderTotal > rewardPointsBalance)
                    {
                        TempData["PaymentMsg"] = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(order.OrderTotal).ToString("N0")
                                                                                         + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
                        return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=2");
                    }
                    else
                    {
                        //TODO : Reward point
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
                    return Redirect(Url.RouteUrl("_Sh_BillAndPayment") + "?orderIds[0]=" + orderId + "&msg=3");

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
            else if (!isForeign && isCod)
            {
                order.OrderStatusId = (int)OrderStatus.Processing;
                _orderService.UpdateOrder(order);
            }
            return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
        }
        public void AddtionalDataForginRequest(Order order)
        {
            //if (_newCheckout.canAddValueToForginRequest(order.Id))
            //{
            //    int orderValue = _newCheckout.getForginAddtionalValue(order.Id);
            //    order.OrderTotal += orderValue;
            //    _orderService.UpdateOrder(order);
            //}
        }
        public IActionResult ChargeWalletIndex()
        {
            if (!IsValidCustomer())
            {
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up" });
            }
            var model = _newCheckout.getPaymentMethodForChargeWallet();
            return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");
        }
        [HttpPost]
        public IActionResult ChargeWallet(string paymentmethod, int amount)
        {
            TempData["PaymentMsg"] = null;
            if (!IsValidCustomer())
            {
                TempData["PaymentMsg"] = "ابتدا وارد سیستم شوید سپس اقدام به پرداخت کنید";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
            }
            if (string.IsNullOrEmpty(paymentmethod))
            {
                TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید

            }
            if (amount <= 0)
            {
                TempData["PaymentMsg"] = "مبلغ وارد شده نامعتبر می باشد";
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
            }
            var checkoutReult = _newCheckout.ProccessWalletOrder(amount, paymentmethod, _workContext.CurrentCustomer.Id);
            if (!checkoutReult.Success)
            {
                TempData["PaymentMsg"] = string.Join("\r\n", checkoutReult.Errors);
                return Redirect(Url.RouteUrl("Nop.Plugin.Misc.PostbarDashboard.Dashboard") + "#list-up");// TODO: باید تغییر کنید
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

        public IActionResult ShowPreOrderInfo(int PreOrderId)
        {
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/PreOrderInfo.cshtml");
        }

        //[HttpPost]
        //public IActionResult ConfirmAndPaySaleCarton(int OrderCode, SendItemSaleKarton List_PP, string paymentmethod, bool UseRewardPoints)
        //{
        //    int amount=0;
        //    #region check params
        //    TempData["PaymentMsg"] = null;
        //    if (!IsValidCustomer())
        //    {
        //        TempData["PaymentMsg"] = "ابتدا وارد سیستم شوید سپس اقدام به پرداخت کنید";
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //    }
        //    if (string.IsNullOrEmpty(paymentmethod))
        //    {
        //        TempData["PaymentMsg"] = "روش پرداخت انتخابی نامعتبر می باشد";
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید

        //    }
        //    if (List_PP.Listitem.Count() == 0)
        //    {
        //        //return Json(new { success = false, responseText = "آیتمی در جدول کالا وجود نداشت" });
        //        TempData["PaymentMsg"] = "آیتمی در جدول کالا وجود نداشت";
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //    }
        //    //check count
        //    bool checkcount = false;
        //    foreach (var item in List_PP.Listitem)
        //    {
        //        if (item.Count > 0)
        //        {
        //            checkcount = true;
        //        }
        //    }
        //    if (checkcount == false)
        //    {
        //        //return Json(new { success = false, responseText = "تعداد کالا ها صفر میباشد، لطفا تعداد مورد نیاز را وارد نمایید" });
        //        TempData["PaymentMsg"] = "تعداد کالا ها صفر میباشد، لطفا تعداد مورد نیاز را وارد نمایید";
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //    }
        //    //check order code
        //    var order = _orderService.GetOrderById(OrderCode);
        //    if (order == null)
        //    {
        //        //return Json(new { success = false, responseText = "شماره سفارش وارد شده معتبر نمیباشد" });
        //        TempData["PaymentMsg"] = "شماره سفارش وارد شده معتبر نمیباشد";
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //    }

        //    /// به دست اورن مبلغ سفارش
        //    foreach (var item in List_PP.Listitem)
        //    {
        //        if (item.Count > 0)
        //        {
        //            var itemq = _repositoryTbl_ProductAttributeValue.GetById(item.Id);
        //            int sum =Convert.ToInt32( item.Count * itemq.PriceAdjustment);
        //            amount += sum;
        //        }
        //    }
        //    ///
        //    if (UseRewardPoints)
        //    {
        //        int rewardPointsBalance =
        //                  _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
        //        if (amount > rewardPointsBalance)
        //        {
        //            TempData["PaymentMsg"] = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(amount).ToString("N0")
        //                                                                             + " ريال باشد. موجودی فعلی " + rewardPointsBalance.ToString("N0");
        //            return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //        }
        //        else
        //        {
        //            // کم کردن از کیف پول
        //            //_rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -Convert.ToInt32(order.OrderTotal), order.StoreId,
        //            //string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
        //            //order, order.OrderTotal);
        //            //order.OrderTotal = 0;
        //            //order.PaymentMethodSystemName = null;
        //            //_orderProcessingService.MarkOrderAsPaid(order);
        //            //return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });

        //        }
        //    }


        //    #endregion
        //    var checkoutReult = _newCheckout.ProccessCartonOrder(amount, paymentmethod, _workContext.CurrentCustomer.Id);
        //    if (!checkoutReult.Success)
        //    {
        //        TempData["PaymentMsg"] = string.Join("\r\n", checkoutReult.Errors);
        //        return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
        //    }
        //    var postProcessPaymentRequest = new PostProcessPaymentRequest
        //    {
        //        Order = checkoutReult.PlacedOrder
        //    };
        //    _paymentService.PostProcessPayment(postProcessPaymentRequest);

        //    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
        //    {
        //        //redirection or POST has been done in PostProcessPayment
        //        return Content("در حال انتقال به درگاه");
        //    }

        //    return RedirectToRoute("CheckoutCompleted", new { orderId = checkoutReult.PlacedOrder.Id });
        //}

        public IActionResult PayForOrderIndex(int orderId)
        {
            if (orderId <= 0)
            {
                return Json(new { message = "اطلاعات ارسالی ناقص می باشد" });
            }
            var order = _orderService.GetOrderById(orderId);
            var PaymentMerhod = _newCheckout.getPaymentMethod(order);
            var model = new OrderBillAndPaymentModel()
            {
                order = order,
                PaymentMethods = PaymentMerhod
            };
            return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/PayForOrder.cshtml", model);
        }
        public IActionResult PayForSaleCarton(int OrderCode, SendItemSaleKarton List_PP)
        {
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return Challenge();


            //if (List_PP.Listitem.Count() == 0)
            //{
            //    //return Json(new { success = false, responseText = "آیتمی در جدول کالا وجود نداشت" });
            //    TempData["PaymentMsg"] = "آیتمی در جدول کالا وجود نداشت";
            //    return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
            //}
            ////check count
            //bool checkcount = false;
            //foreach (var item in List_PP.Listitem)
            //{
            //    if (item.Count > 0)
            //    {
            //        checkcount = true;
            //    }
            //}
            //if (checkcount == false)
            //{
            //    //return Json(new { success = false, responseText = "تعداد کالا ها صفر میباشد، لطفا تعداد مورد نیاز را وارد نمایید" });
            //    TempData["PaymentMsg"] = "تعداد کالا ها صفر میباشد، لطفا تعداد مورد نیاز را وارد نمایید";
            //    return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید
            //}
            ////check order code
            //var order = _orderService.GetOrderById(OrderCode);
            //if (order == null)
            //{
            //    //return Json(new { success = false, responseText = "شماره سفارش وارد شده معتبر نمیباشد" });
            //    TempData["PaymentMsg"] = "شماره سفارش وارد شده معتبر نمیباشد";
            //    return Redirect(Url.RouteUrl("_Sh_ConfirmAndPaySaleCarton"));// TODO: باید تغییر کنید

            //}
            //var PaymentMethod = _newCheckout.getPaymentMethodForSaleCarton(OrderCode, List_PP);
            //var model = new vm_SaleCartonWrapper()
            //{
            //    Listitem = List_PP.Listitem,
            //    PaymentMethods = PaymentMethod
            //};
            return View("~/Plugins/Misc.PrintedReports_Requirements/Views/SaleCartonwrapper/PayForOrder.cshtml");//, model
        }

        [HttpGet]
        public IActionResult IsInInvalidService(int countryId, int stateId)
        {
            var _IsInvalid = _newCheckout.isInvalidSernder(countryId, stateId);
            return Json(new { isInvalid = _IsInvalid });
        }
    }
}
