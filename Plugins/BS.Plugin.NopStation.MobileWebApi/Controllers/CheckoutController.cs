using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.PreOrderModel;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService;
using Nop.Plugin.Orders.BulkOrder.Models;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CheckoutController : BaseApiController
    {
        #region field
        private readonly ICartonService _cartonService;
        private readonly IDbContext _dbContext;
        private readonly INewCheckout _newCheckout;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IBulkOrderService _bulkOrderService;
        private readonly IReOrderService _reOrderService;
        private readonly IApiOrderRefrenceCodeService _apiOrderRefrenceCodeService;
        private readonly ICheckoutModelFactoryApi _checkoutModelFactoryApi;
        private readonly IWorkContext _workContext;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly AddressSettings _addressSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IShippingService _shippingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPaymentService _paymentService;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly AuthorizeNetPaymentSettings _authorizeNetPaymentSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly IRewardPointService _rewardPointService;
        private readonly IPreOrderService _preOrderService;
        private readonly ICodService _codService;
        private static string LockCustomer = "";
        #endregion

        #region Ctor
        public CheckoutController(
            ICartonService cartonService,
            IPreOrderService preOrderService,
            IDbContext dbContext,
            IExtendedShipmentService extendedShipmentService,
            ICheckoutModelFactoryApi checkoutModelFactoryApi,
            IWorkContext workContext, IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            ILocalizationService localizationService, AddressSettings addressSettings,
            IStoreContext storeContext, ICustomerService customerService,
            ILogger logger, IShippingService shippingService,
            IGenericAttributeService genericAttributeService,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceFormatter priceFormatter, ShippingSettings shippingSettings,
            PaymentSettings paymentSettings, IPaymentService paymentService,
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings, IOrderService orderService,
            IOrderProcessingService orderProcessingService, IPluginFinder pluginFinder,
            AuthorizeNetPaymentSettings authorizeNetPaymentSettings,
            CurrencySettings currencySettings,
            IBulkOrderService bulkOrderService,
            IReOrderService reOrderService,
            INewCheckout newCheckout,
            IWebHelper webHelper,
            IRewardPointService rewardPointService,
            IApiOrderRefrenceCodeService apiOrderRefrenceCodeService, ICodService codService)
        {
            _cartonService = cartonService;
            _preOrderService = preOrderService;
            _rewardPointService = rewardPointService;
            this._dbContext = dbContext;
            _extendedShipmentService = extendedShipmentService;
            this._bulkOrderService = bulkOrderService;
            _reOrderService = reOrderService;
            _apiOrderRefrenceCodeService = apiOrderRefrenceCodeService;
            this._checkoutModelFactoryApi = checkoutModelFactoryApi;
            this._workContext = workContext;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._addressSettings = addressSettings;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._logger = logger;
            this._shippingService = shippingService;
            this._genericAttributeService = genericAttributeService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._shippingSettings = shippingSettings;
            this._paymentSettings = paymentSettings;
            this._paymentService = paymentService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._pluginFinder = pluginFinder;
            this._authorizeNetPaymentSettings = authorizeNetPaymentSettings;
            this._currencySettings = currencySettings;
            this._newCheckout = newCheckout;
            this._webHelper = webHelper;
            LockCustomer = "customer_" + _workContext.CurrentCustomer.Id;
            _codService = codService;
        }
        #endregion

        #region New Action Method
        [Route("api/checkout/bulkNewOrder")]
        [HttpPost]
        public IActionResult BulkCheckoutOrder(List<CheckoutItemApi> model)
        {
            if (!IsValidCustomer())
                return Json(new { resultCode = 16, message = ApiMessage.GetErrorMsg(16) });
            if (model == null)
                return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
            if (!model.Any())
                return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
            if (model.Any(p => p.IsCOD) && model.Any(p => !p.IsCOD))
                return Json(new { resultCode = 12, message = ApiMessage.GetErrorMsg(12) });
            string error = "";
            int resultCode = 0;
            foreach (var item in model)
            {
                resultCode = item.IsValid(out error);
                if (resultCode != 0)
                {
                    //Todo  add Log
                    return Json(new { resultCode = resultCode, message = error });
                }
            }

            var placeOrderResult = _bulkOrderService.ProcessOrderList(apiModel: model, customerId: _workContext.CurrentCustomer.Id, discountCouponCode: model[0].discountCouponCode);
            if (placeOrderResult.Success)
            {
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = placeOrderResult.PlacedOrder
                };
                _paymentService.PostProcessPayment(postProcessPaymentRequest);
                var orderId = placeOrderResult.PlacedOrder.Id;
                return Json(new { resultCode = 0, message = ApiMessage.GetErrorMsg(0), orderId = orderId });
            }

            string warning = string.Join(",", placeOrderResult.Errors);
            if (warning.Contains("erroCode:930"))
            {
                warning.Replace("erroCode:930", "");
                resultCode = 10;
            }
            if (warning.Contains("erroCode:940"))
            {
                warning.Replace("erroCode:940", "");
                resultCode = 11;
            }
            return Json(new { resultCode, message = ApiMessage.GetErrorMsg(9) + "\r\n" + warning });
        }
        [Route("api/checkout/newOrder")]
        [HttpPost]
        public IActionResult CheckoutOrder(CheckoutItemApi model)
        {
            if (_workContext.CurrentCustomer.IsInCustomerRole("Nasher"))
                return _GhasedakCheckoutOrder(model);
            return _checkoutOrder(model);
        }
        [Route("api/checkout/jNewOrder")]
        [HttpPost]
        public IActionResult JsonCheckoutOrder([FromBody] CheckoutItemApi model)
        {
            if (_workContext.CurrentCustomer.IsInCustomerRole("Nasher"))
                return _GhasedakCheckoutOrder(model);
            return _checkoutOrder(model);
        }
        [Route("api/checkout/RegisterOrder")]
        [HttpPost]
        public IActionResult RegisterOrder([FromBody] CheckoutParcellModel model)
        {
            try
            {
                string url = "";
                if (model == null)
                    return Json(new { message = "فرمت اطلاعات ارسالی نا معتبر می باشد", success = false });
                string error = "";
                if (!model.IsValid(out error))
                    return Json(new { message = error, success = false });
                int preOrderId = _preOrderService.InsertPreOrder(model, _workContext.CurrentCustomer.Id, out error, out url);
                if (preOrderId == 0)
                    return Json(new { message = error, success = false });
                return Json(new { message = "ثبت با موفقیت انجام شد", success = true, PreOrderId = preOrderId, Url = url });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { message = "خطا در زمان ثبت اطلاعات", success = false });
            }
        }
        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        /// <summary>
        /// تکمیل سفارش های ارسال به پست نامتقارن 
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [Route("api/checkout/CompleteAsyncOrder")]
        public IActionResult CompleteAsyncSendToPostOrder(int OrderId)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsInCustomerRole("asyncSendToPost"))
                {
                    return Json(new { resultCode = 1, message = "امکان استفاده از این سرویس برای شما فعال  نمی باشد" });
                }
                var order = _orderService.GetOrderById(OrderId);
                if (order == null)
                {
                    return Json(new { resultCode = 1, message = "شماره سفارش وارد شده نامعتبر است" });
                }
                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                {
                    return Json(new { resultCode = 1, message = "این سفارش متعلق به شما نیست" });
                }
                var orderNote = order.OrderNotes.FirstOrDefault(x => x.Note == "PaymentHandled");
                if (order.PaymentStatus == PaymentStatus.Paid && order.OrderStatus != OrderStatus.Cancelled && orderNote != null)
                {
                    _orderService.DeleteOrderNote(orderNote);
                    if (order.OrderStatus == OrderStatus.Pending)
                    {
                        order.OrderStatus = OrderStatus.Complete;
                        _orderService.UpdateOrder(order);
                    }
                }
                return Json(new { resultCode = 0, message = "عملیات با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                return Json(new { resultCode = -1, message = ex.Message });
            }
        }

        private IActionResult _checkoutOrder(CheckoutItemApi model)
        {
            lock ("customer_" + _workContext.CurrentCustomer.Id)
            {
                if (_storeContext.CurrentStore.Id == 3)
                {
                    return Json(new { resultCode = 40, message = "سرویس مورد نظر غیر فعال شده. لطفا جهت اطلاعات بیشتر با واحد پشتیبانی تماس بگیرید" });
                }
                if (_workContext.CurrentCustomer.Id != 4144899 && _workContext.CurrentCustomer.Id != 11295490)
                {
                    if (model.SenderLat == null || model.SenderLon == null)
                    {
                        return Json(new { resultCode = 21, message = ApiMessage.GetErrorMsg(21) });
                    }
                    if (model.NeedCarton && (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم."))
                    {
                        return Json(new { resultCode = 50, message = "کاربر گرامی باتوجه به درخواست شما برای کارتن و بسته بندی لطفا سایز استاندار مورد نظر را اعلام بفرمایید " });
                    }

                    if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                          (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                    {
                        if ((model.CartonSizeName == "سایر(بزرگتر از سایز 9)"))
                        {
                            return Json(new { resultCode = 51, message = "لطفا ابعاد مرسوله را به درستی وارد کنید" });
                        }

                        if (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم.")
                        {
                            model.CartonSizeName = "سایز 4(20*20*30)";
                        }
                    }
                }

                if (!IsValidCustomer())
                    return Json(new { resultCode = 16, message = ApiMessage.GetErrorMsg(16) });
                if (model == null)
                    return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
                if (_extendedShipmentService.isInvalidSender(model.Sender_StateId, model.Sender_townId))
                {
                    return Json(new { resultCode = 18, message = ApiMessage.GetErrorMsg(18) });
                }
                string error = "";
                int resultCode = model.IsValid(out error);
                if (resultCode != 0)
                {
                    //Todo  add Log
                    return Json(new { resultCode = resultCode, message = error });
                }


                if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                {
                    lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                    {
                        if (!_apiOrderRefrenceCodeService.CheckAndInsertApiOrderRefrenceCode(_workContext.CurrentCustomer.Id, model.refrenceNo, out Tbl_ApiOrderRefrenceCode tbl_ApiOrderRefrenceCode))
                        {
                            return Json(new { resultCode = 19, message = string.Format(ApiMessage.GetErrorMsg(19), model.refrenceNo, tbl_ApiOrderRefrenceCode.OrderId.Value), orderId = tbl_ApiOrderRefrenceCode.OrderId.Value });
                        }
                    }
                }
                string _CartonSizeName = model.CartonSizeName;
                if (!model.NeedCarton)
                {
                    model.CartonSizeName = "کارتن نیاز ندارم.";
                }
                if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                            (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                {
                    var dimentions = _newCheckout.getDimentionByName(_CartonSizeName);
                    if (dimentions != null)
                    {
                        model.width = dimentions.Width;
                        model.height = dimentions.Height;
                        model.length = dimentions.Length;
                    }

                }
                #region CreateSpModel
                NewCheckoutModel ApiCheckoutModel = new NewCheckoutModel()
                {
                    AgentSaleAmount = model.AgentSaleAmount,
                    boxType = (model.boxType == "0" ? "پاکت" : "بسته"),
                    CartonSizeName = model.CartonSizeName,
                    CodGoodsPrice = model.CodGoodsPrice,
                    Count = model.Count,
                    discountCouponCode = model.discountCouponCode,
                    dispatch_date = model.dispatch_date,
                    //getItNow 
                    GoodsType = model.GoodsType,
                    HasAccessToPrinter = model.HasAccessToPrinter,
                    hasNotifRequest = model.notifBySms,
                    height = model.height,
                    width = model.width,
                    length = model.length,
                    InsuranceName = model.InsuranceName,
                    IsCOD = model.IsCOD,
                    receiver_ForeginCityName = model.receiver_ForeginCityName,
                    receiver_ForeginCountry = model.receiver_ForeginCountry.GetValueOrDefault(0),
                    receiver_ForeginCountryName = model.receiver_ForeginCountryName,
                    receiver_ForeginCountryNameEn = model.receiver_ForeginCountryName,
                    ApproximateValue = model.ApproximateValue,
                    RequestPrintAvatar = model.printLogo,
                    ReciverLat = model.ReciverLat,
                    ReciverLon = model.ReciverLon,
                    SenderLat = model.SenderLat,
                    SenderLon = model.SenderLon,
                    UbbarPackingLoad = model.UbbarPackingLoad,
                    ServiceId = model.ServiceId,
                    _dispatch_date = model._dispatch_date,
                    Weight = model.Weight,
                    VechileOptions = model.VechileOptions,
                    UbbraTruckType = model.UbbraTruckType,
                    needCaton = model.NeedCarton
                };
                ApiCheckoutModel.billingAddressModel = new Address()
                {
                    Address1 = model.Sender_Address,
                    City = model.Sender_City,
                    CountryId = model.Sender_StateId,
                    StateProvinceId = model.Sender_townId,
                    Email = model.Sender_Email,
                    FirstName = model.Sender_FristName,
                    LastName = model.Sender_LastName,
                    PhoneNumber = model.Sender_mobile,
                    ZipPostalCode = model.Sender_PostCode
                };
                ApiCheckoutModel.shippingAddressModel = new Address()
                {
                    Address1 = model.Reciver_Address,
                    City = model.Reciver_City,
                    CountryId = model.Reciver_StateId,
                    StateProvinceId = model.Reciver_townId,
                    Email = model.Reciver_Email,
                    FirstName = model.Reciver_FristName,
                    LastName = model.Reciver_LastName,
                    PhoneNumber = model.Reciver_mobile,
                    ZipPostalCode = model.Reciver_PostCode
                };
                List<NewCheckoutModel> Lst_Checkout = new List<NewCheckoutModel>();
                Lst_Checkout.Add(ApiCheckoutModel);
                #endregion

                string resultMessage = "";
                List<string> strError = new List<string>();
                var inputModel = new NewCheckout_Sp_Input()
                {
                    JsonOrderList = JsonConvert.SerializeObject(Lst_Checkout),
                    JsonData = JsonConvert.SerializeObject(new
                    {
                        CustommerId = _workContext.CurrentCustomer.Id,
                        CustommerIp = _webHelper.GetCurrentIpAddress(),
                        StoreId = _storeContext.CurrentStore.Id,
                        IsWebApi = true,
                        SourceId = (model.orderSource <= 0 ? 3 : model.orderSource)
                    })
                };
                try
                {
                    var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Api, 0, false, false);
                    if (ret.orderId > 0)
                    {
                        var order = _orderService.GetOrderById(ret.orderId);
                        _reOrderService.InsertOrderJson(order.Id, JsonConvert.SerializeObject(model), true);
                        if (_workContext.CurrentCustomer.AffiliateId == 1149)
                        {
                            _extendedShipmentService._GenerateBarcodes(order, out strError);
                            if (strError.Count() > 0)
                            {
                                resultMessage = ApiMessage.GetErrorMsg(17) + "\r\n" + string.Join("\r\n", strError);
                                return Json(new { resultCode = 17, message = resultMessage, orderId = order.Id });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                        {
                            lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                            {
                                _apiOrderRefrenceCodeService.SetOrderId(_workContext.CurrentCustomer.Id, model.refrenceNo, ret.orderId);
                            }
                        }
                        if (!IsOkOrder(order.Id) && _orderProcessingService.CanCancelOrder(order))
                        {
                            _orderProcessingService.CancelOrder(order, false);
                            resultMessage = ApiMessage.GetErrorMsg(22) + "\r\n" + string.Join("\r\n", strError);
                            return Json(new { resultCode = 22, message = resultMessage, orderId = order.Id });
                        }
                        resultMessage = ApiMessage.GetErrorMsg(0) + "\r\n" + string.Join("\r\n", strError);
                        int orderTotal = Convert.ToInt32(order.OrderTotal);
                        if (order.OrderTotal == 0 && order.PaymentMethodSystemName == null)
                        {
                            if (order.RedeemedRewardPointsEntry != null)
                            {
                                orderTotal = order.RedeemedRewardPointsEntry.Points * -1;
                            }
                        }
                        return Json(new { resultCode = 0, message = resultMessage, orderId = order.Id, orderTotal = Convert.ToInt32(orderTotal) });
                    }
                    return Json(new { message = ret.ErrorMessage, success = ret.ErrorCode == 0, orderIds = ret.orderId });
                }
                catch (Exception ex)
                {
                    return Json(new { message = ex.Message, success = false });
                }

            }

        }
        [Route("api/checkout/panelNewOrder")]
        [HttpPost]
        public IActionResult Panel_checkoutOrder([FromBody] CheckoutItemApi model)
        {
            if (model == null)
            {
                return Json(new { resultCode = 30, message = "اطلاعات مورد نظر دریافت نشد" });
            }
            lock ("customer_" + _workContext.CurrentCustomer.Id)
            {
                int _serviceId = 0;
                if (_storeContext.CurrentStore.Id == 3)
                {
                    return Json(new { resultCode = 40, message = "سرویس مورد نظر غیر فعال شده. لطفا جهت اطلاعات بیشتر با واحد پشتیبانی تماس بگیرید" });
                }
                if (_workContext.CurrentCustomer.Id != 4144899 && _workContext.CurrentCustomer.Id != 11295490)
                {
                    if (model.SenderLat == null || model.SenderLon == null)
                    {
                        return Json(new { resultCode = 21, message = ApiMessage.GetErrorMsg(21) });
                    }
                    if (model.NeedCarton && (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم."))
                    {
                        return Json(new { resultCode = 50, message = "کاربر گرامی باتوجه به درخواست شما برای کارتن و بسته بندی لطفا سایز استاندار مورد نظر را اعلام بفرمایید " });
                    }

                    if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                          (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                    {
                        if ((model.CartonSizeName == "سایر(بزرگتر از سایز 9)"))
                        {
                            return Json(new { resultCode = 51, message = "لطفا ابعاد مرسوله را به درستی وارد کنید" });
                        }

                        if (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم.")
                        {
                            model.CartonSizeName = "سایز 4(20*20*30)";
                        }
                    }
                }
                if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                          (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0))
                          && !string.IsNullOrEmpty(model.CartonSizeName) && model.CartonSizeName != "کارتن نیاز ندارم.")
                {
                    var cartondate=_cartonService.getcartonInfoBySizeName(model.CartonSizeName);
                    if (cartondate != null)
                    {
                        model.width = Convert.ToInt32(cartondate.Width);
                        model.height = Convert.ToInt32(cartondate.Height);
                        model.length = Convert.ToInt32(cartondate.Length);
                    }
                }
                if (!IsValidCustomer())
                    return Json(new { resultCode = 16, message = ApiMessage.GetErrorMsg(16) });
                if (model == null)
                    return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
                if (_extendedShipmentService.isInvalidSender(model.Sender_StateId, model.Sender_townId))
                {
                    return Json(new { resultCode = 18, message = ApiMessage.GetErrorMsg(18) });
                }
                string error = "";
                int resultCode = model.IsValid(out error);
                if (resultCode != 0)
                {
                    //Todo  add Log
                    return Json(new { resultCode = resultCode, message = error });
                }


                if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                {
                    lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                    {
                        if (!_apiOrderRefrenceCodeService.CheckAndInsertApiOrderRefrenceCode(_workContext.CurrentCustomer.Id, model.refrenceNo, out Tbl_ApiOrderRefrenceCode tbl_ApiOrderRefrenceCode))
                        {
                            return Json(new { resultCode = 19, message = string.Format(ApiMessage.GetErrorMsg(19), model.refrenceNo, tbl_ApiOrderRefrenceCode.OrderId.Value), orderId = tbl_ApiOrderRefrenceCode.OrderId.Value });
                        }
                    }
                }
                string _CartonSizeName = model.CartonSizeName;
                if (!model.NeedCarton)
                {
                    model.CartonSizeName = "کارتن نیاز ندارم.";
                }
                if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                            (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                {
                    var dimentions = _newCheckout.getDimentionByName(_CartonSizeName);
                    if (dimentions != null)
                    {
                        model.width = dimentions.Width;
                        model.height = dimentions.Height;
                        model.length = dimentions.Length;
                    }

                }
                #region CreateSpModel
                NewCheckoutModel ApiCheckoutModel = new NewCheckoutModel()
                {
                    AgentSaleAmount = model.AgentSaleAmount,
                    boxType = (model.boxType == "0" ? "پاکت" : "بسته"),
                    CartonSizeName = model.CartonSizeName,
                    CodGoodsPrice = model.CodGoodsPrice,
                    Count = model.Count,
                    discountCouponCode = model.discountCouponCode,
                    dispatch_date = model.dispatch_date,
                    //getItNow 
                    GoodsType = model.GoodsType,
                    HasAccessToPrinter = model.HasAccessToPrinter,
                    hasNotifRequest = model.notifBySms,
                    height = model.height,
                    width = model.width,
                    length = model.length,
                    InsuranceName = model.InsuranceName,
                    IsCOD = model.IsCOD,
                    IsFreePost = model.IsFreePost,
                    receiver_ForeginCityName = model.receiver_ForeginCityName,
                    receiver_ForeginCountry = model.receiver_ForeginCountry.GetValueOrDefault(0),
                    receiver_ForeginCountryName = model.receiver_ForeginCountryName,
                    receiver_ForeginCountryNameEn = model.receiver_ForeginCountryName,
                    ApproximateValue = model.ApproximateValue,
                    RequestPrintAvatar = model.printLogo,
                    ReciverLat = model.ReciverLat,
                    ReciverLon = model.ReciverLon,
                    SenderLat = model.SenderLat,
                    SenderLon = model.SenderLon,
                    UbbarPackingLoad = model.UbbarPackingLoad,
                    ServiceId = model.ServiceId,
                    _dispatch_date = model._dispatch_date,
                    Weight = model.Weight,
                    VechileOptions = model.VechileOptions,
                    UbbraTruckType = model.UbbraTruckType,
                    needCaton = model.NeedCarton
                };
                _serviceId = model.ServiceId;
                ApiCheckoutModel.billingAddressModel = new Address()
                {
                    Address1 = model.Sender_Address,
                    City = model.Sender_City,
                    CountryId = model.Sender_StateId,
                    StateProvinceId = model.Sender_townId,
                    Email = model.Sender_Email,
                    FirstName = model.Sender_FristName,
                    LastName = model.Sender_LastName,
                    PhoneNumber = model.Sender_mobile,
                    ZipPostalCode = model.Sender_PostCode
                };
                ApiCheckoutModel.shippingAddressModel = new Address()
                {
                    Address1 = model.Reciver_Address,
                    City = model.Reciver_City,
                    CountryId = model.Reciver_StateId,
                    StateProvinceId = model.Reciver_townId,
                    Email = model.Reciver_Email,
                    FirstName = model.Reciver_FristName,
                    LastName = model.Reciver_LastName,
                    PhoneNumber = model.Reciver_mobile,
                    ZipPostalCode = model.Reciver_PostCode
                };
                List<NewCheckoutModel> Lst_Checkout = new List<NewCheckoutModel>();
                Lst_Checkout.Add(ApiCheckoutModel);
                #endregion

                string resultMessage = "";
                List<string> strError = new List<string>();
                var inputModel = new NewCheckout_Sp_Input()
                {
                    JsonOrderList = JsonConvert.SerializeObject(Lst_Checkout),
                    JsonData = JsonConvert.SerializeObject(new
                    {
                        CustommerId = _workContext.CurrentCustomer.Id,
                        CustommerIp = _webHelper.GetCurrentIpAddress(),
                        StoreId = _storeContext.CurrentStore.Id,
                        IsWebApi = true,
                        SourceId = (model.orderSource <= 0 ? 3 : model.orderSource)
                    })
                };
                try
                {
                    var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Api, 0, false, false);
                    if (ret.orderId > 0)
                    {
                        var order = _orderService.GetOrderById(ret.orderId);
                        _reOrderService.InsertOrderJson(order.Id, JsonConvert.SerializeObject(model), true);
                        if (_workContext.CurrentCustomer.AffiliateId == 1149)
                        {
                            _extendedShipmentService._GenerateBarcodes(order, out strError);
                            if (strError.Count() > 0)
                            {
                                resultMessage = ApiMessage.GetErrorMsg(17) + "\r\n" + string.Join("\r\n", strError);
                                return Json(new { resultCode = 17, message = resultMessage, orderId = order.Id });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                        {
                            lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                            {
                                _apiOrderRefrenceCodeService.SetOrderId(_workContext.CurrentCustomer.Id, model.refrenceNo, ret.orderId);
                            }
                        }
                        if (!IsOkOrder(order.Id) && _orderProcessingService.CanCancelOrder(order))
                        {
                            _orderProcessingService.CancelOrder(order, false);
                            resultMessage = ApiMessage.GetErrorMsg(22) + "\r\n" + string.Join("\r\n", strError);
                            return Json(new { resultCode = 22, message = resultMessage, orderId = order.Id });
                        }
                        resultMessage = ApiMessage.GetErrorMsg(0) + "\r\n" + string.Join("\r\n", strError);
                        int orderTotal = Convert.ToInt32(order.OrderTotal);
                        if (order.OrderTotal == 0 && order.PaymentMethodSystemName == null)
                        {
                            if (order.RedeemedRewardPointsEntry != null)
                            {
                                orderTotal = order.RedeemedRewardPointsEntry.Points * -1;
                            }
                        }
                        if (order.PaymentMethodSystemName == "Payments.CashOnDelivery" && order.OrderStatus == OrderStatus.Pending)
                        {
                            order.OrderStatus = OrderStatus.Processing;
                            _orderService.UpdateOrder(order);
                        }
                        return Json(new PlaceOrderResult
                        {
                            resultCode = 0,
                            message = resultMessage,
                            orderId = order.Id,
                            success = true,
                            orderTotal = Convert.ToInt32(orderTotal),
                            shipmentsData = getShipmentsData(ret.orderId, _serviceId, model.IsFreePost, model.orderSource)
                        });
                    }
                    return Json(new PlaceOrderResult
                    {

                        message = ret.ErrorMessage,
                        success = ret.ErrorCode == 0,
                        orderId = ret.orderId,
                        orderTotal = 0,
                        shipmentsData = getShipmentsData(ret.orderId, _serviceId, model.IsFreePost, model.orderSource),
                        resultCode = (ret.ErrorCode != 0 ? 9 : 0)
                    });
                }
                catch (Exception ex)
                {
                    return Json(new PlaceOrderResult { message = ex.Message, success = false, resultCode = 9 });
                }

            }

        }
        public class PlaceOrderResult
        {
            public string message { get; set; }
            public bool success { get; set; }
            public int orderId { get; set; }
            public int orderTotal { get; set; }
            public List<ShipmentsData> shipmentsData { get; set; }
            public int resultCode { get; set; }
        }
        [Route("api/checkout/panelNewOrderList")]
        [HttpPost]
        public IActionResult Panel_checkoutOrderList([FromBody] List<CheckoutItemApi> Lst_model)
        {
            List<NewCheckoutModel> Lst_Checkout = new List<NewCheckoutModel>();

            lock ("customer_" + _workContext.CurrentCustomer.Id)
            {
                int _serviceId = 0;
                if (_storeContext.CurrentStore.Id == 3)
                {
                    return Json(new { resultCode = 40, message = "سرویس مورد نظر غیر فعال شده. لطفا جهت اطلاعات بیشتر با واحد پشتیبانی تماس بگیرید" });
                }

                foreach (var model in Lst_model)
                {


                    if (_workContext.CurrentCustomer.Id != 4144899 && _workContext.CurrentCustomer.Id != 11295490)
                    {

                        if (model.SenderLat == null || model.SenderLon == null)
                        {
                            return Json(new PlaceOrderResult { resultCode = 21, message = ApiMessage.GetErrorMsg(21) });
                        }
                        if (model.NeedCarton && (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم."))
                        {
                            return Json(new PlaceOrderResult { resultCode = 50, message = "کاربر گرامی باتوجه به درخواست شما برای کارتن و بسته بندی لطفا سایز استاندار مورد نظر را اعلام بفرمایید " });
                        }

                        if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                              (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                        {
                            if ((model.CartonSizeName == "سایر(بزرگتر از سایز 9)"))
                            {
                                return Json(new PlaceOrderResult { resultCode = 51, message = "لطفا ابعاد مرسوله را به درستی وارد کنید" });
                            }

                            if (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم.")
                            {
                                model.CartonSizeName = "سایز 4(20*20*30)";
                            }
                        }
                    }

                    if (!IsValidCustomer())
                        return Json(new PlaceOrderResult { resultCode = 16, message = ApiMessage.GetErrorMsg(16) });
                    if (model == null)
                        return Json(new PlaceOrderResult { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
                    if (_extendedShipmentService.isInvalidSender(model.Sender_StateId, model.Sender_townId))
                    {
                        return Json(new PlaceOrderResult { resultCode = 18, message = ApiMessage.GetErrorMsg(18) });
                    }
                    string error = "";
                    int resultCode = model.IsValid(out error);
                    if (resultCode != 0)
                    {
                        //Todo  add Log
                        return Json(new PlaceOrderResult { resultCode = resultCode, message = error });
                    }


                    if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                    {
                        lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                        {
                            if (!_apiOrderRefrenceCodeService.CheckAndInsertApiOrderRefrenceCode(_workContext.CurrentCustomer.Id, model.refrenceNo, out Tbl_ApiOrderRefrenceCode tbl_ApiOrderRefrenceCode))
                            {
                                return Json(new PlaceOrderResult { resultCode = 19, message = string.Format(ApiMessage.GetErrorMsg(19), model.refrenceNo, tbl_ApiOrderRefrenceCode.OrderId.Value), orderId = tbl_ApiOrderRefrenceCode.OrderId.Value });
                            }
                        }
                    }
                    string _CartonSizeName = model.CartonSizeName;
                    if (!model.NeedCarton)
                    {
                        model.CartonSizeName = "کارتن نیاز ندارم.";
                    }
                    if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                                (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0)))
                    {
                        var dimentions = _newCheckout.getDimentionByName(_CartonSizeName);
                        if (dimentions != null)
                        {
                            model.width = dimentions.Width;
                            model.height = dimentions.Height;
                            model.length = dimentions.Length;
                        }

                    }
                    #region CreateSpModel
                    NewCheckoutModel ApiCheckoutModel = new NewCheckoutModel()
                    {
                        AgentSaleAmount = model.AgentSaleAmount,
                        boxType = (model.boxType == "0" ? "پاکت" : "بسته"),
                        CartonSizeName = model.CartonSizeName,
                        CodGoodsPrice = model.CodGoodsPrice,
                        Count = model.Count,
                        discountCouponCode = model.discountCouponCode,
                        dispatch_date = model.dispatch_date,
                        //getItNow 
                        GoodsType = model.GoodsType,
                        HasAccessToPrinter = model.HasAccessToPrinter,
                        hasNotifRequest = model.notifBySms,
                        height = model.height,
                        width = model.width,
                        length = model.length,
                        InsuranceName = model.InsuranceName,
                        IsCOD = model.IsCOD,
                        IsFreePost = model.IsFreePost,
                        receiver_ForeginCityName = model.receiver_ForeginCityName,
                        receiver_ForeginCountry = model.receiver_ForeginCountry.GetValueOrDefault(0),
                        receiver_ForeginCountryName = model.receiver_ForeginCountryName,
                        receiver_ForeginCountryNameEn = model.receiver_ForeginCountryName,
                        ApproximateValue = model.ApproximateValue,
                        RequestPrintAvatar = model.printLogo,
                        ReciverLat = model.ReciverLat,
                        ReciverLon = model.ReciverLon,
                        SenderLat = model.SenderLat,
                        SenderLon = model.SenderLon,
                        UbbarPackingLoad = model.UbbarPackingLoad,
                        ServiceId = model.ServiceId,
                        _dispatch_date = model._dispatch_date,
                        Weight = model.Weight,
                        VechileOptions = model.VechileOptions,
                        UbbraTruckType = model.UbbraTruckType,
                        needCaton = model.NeedCarton,
                        ShipmentTempId = model.shipmentTempId
                    };
                    _serviceId = model.ServiceId;
                    ApiCheckoutModel.billingAddressModel = new Address()
                    {
                        Address1 = model.Sender_Address,
                        City = model.Sender_City,
                        CountryId = model.Sender_StateId,
                        StateProvinceId = model.Sender_townId,
                        Email = model.Sender_Email,
                        FirstName = model.Sender_FristName,
                        LastName = model.Sender_LastName,
                        PhoneNumber = model.Sender_mobile,
                        ZipPostalCode = model.Sender_PostCode
                    };
                    ApiCheckoutModel.shippingAddressModel = new Address()
                    {
                        Address1 = model.Reciver_Address,
                        City = model.Reciver_City,
                        CountryId = model.Reciver_StateId,
                        StateProvinceId = model.Reciver_townId,
                        Email = model.Reciver_Email,
                        FirstName = model.Reciver_FristName,
                        LastName = model.Reciver_LastName,
                        PhoneNumber = model.Reciver_mobile,
                        ZipPostalCode = model.Reciver_PostCode
                    };
                    Lst_Checkout.Add(ApiCheckoutModel);

                    #endregion
                }
                string resultMessage = "";
                List<string> strError = new List<string>();
                var inputModel = new NewCheckout_Sp_Input()
                {
                    JsonOrderList = JsonConvert.SerializeObject(Lst_Checkout),
                    JsonData = JsonConvert.SerializeObject(new
                    {
                        CustommerId = _workContext.CurrentCustomer.Id,
                        CustommerIp = _webHelper.GetCurrentIpAddress(),
                        StoreId = _storeContext.CurrentStore.Id,
                        IsWebApi = true,
                        SourceId = (Lst_model.First().orderSource <= 0 ? 3 : Lst_model.First().orderSource)
                    })
                };
                try
                {
                    var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Api, 0, false, false);
                    if (ret.orderId > 0)
                    {
                        var order = _orderService.GetOrderById(ret.orderId);
                        _reOrderService.InsertOrderJson(order.Id, JsonConvert.SerializeObject(Lst_Checkout), true);
                        if (_workContext.CurrentCustomer.AffiliateId == 1149)
                        {
                            _extendedShipmentService._GenerateBarcodes(order, out strError);
                            if (strError.Count() > 0)
                            {
                                resultMessage = ApiMessage.GetErrorMsg(17) + "\r\n" + string.Join("\r\n", strError);
                                return Json(new PlaceOrderResult { resultCode = 17, message = resultMessage, orderId = order.Id });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Lst_model.First().refrenceNo))
                        {
                            lock (Lst_model.First().refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                            {
                                _apiOrderRefrenceCodeService.SetOrderId(_workContext.CurrentCustomer.Id, Lst_model.First().refrenceNo, ret.orderId);
                            }
                        }
                        if (!IsOkOrder(order.Id) && _orderProcessingService.CanCancelOrder(order))
                        {
                            _orderProcessingService.CancelOrder(order, false);
                            resultMessage = ApiMessage.GetErrorMsg(22) + "\r\n" + string.Join("\r\n", strError);
                            return Json(new PlaceOrderResult { resultCode = 22, message = resultMessage, orderId = order.Id });
                        }
                        resultMessage = ApiMessage.GetErrorMsg(0) + "\r\n" + string.Join("\r\n", strError);
                        int orderTotal = Convert.ToInt32(order.OrderTotal);
                        if (order.OrderTotal == 0 && order.PaymentMethodSystemName == null)
                        {
                            if (order.RedeemedRewardPointsEntry != null)
                            {
                                orderTotal = order.RedeemedRewardPointsEntry.Points * -1;
                            }
                        }
                        if (order.PaymentMethodSystemName == "Payments.CashOnDelivery" && order.OrderStatus == OrderStatus.Pending)
                        {
                            order.OrderStatus = OrderStatus.Processing;
                            _orderService.UpdateOrder(order);
                        }
                        return Json(new PlaceOrderResult
                        {
                            resultCode = 0,
                            message = resultMessage,
                            orderId = order.Id,
                            orderTotal = Convert.ToInt32(orderTotal),
                            shipmentsData = getShipmentsData(ret.orderId, _serviceId, Lst_model[0].IsFreePost, Lst_model[0].orderSource)
                        });
                    }
                    return Json(new PlaceOrderResult { message = ret.ErrorMessage, success = ret.ErrorCode == 0, orderId = ret.orderId, shipmentsData = getShipmentsData(ret.orderId, _serviceId, Lst_model[0].IsFreePost, Lst_model[0].orderSource) });
                }
                catch (Exception ex)
                {
                    return Json(new PlaceOrderResult { message = ex.Message, success = false });
                }

            }

        }
        [Route("api/checkout/PayOrder")]
        [HttpPost]
        public IActionResult PayOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return Json(new { message = "شماره سفارش وارد شده نامعتبر می باشد", success = false, orderIds = orderId, shipmentsData = (List<ShipmentsData>)null });
                var _order = _orderService.GetOrderById(orderId);
                if (_order == null)
                    return Json(new { message = "شماره سفارش وارد شده نامعتبر می باشد", success = false, orderIds = orderId, shipmentsData = (List<ShipmentsData>)null });
                if (_order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Json(new { message = "شماره سفارش وارد شده مطعلق به شخص دیگری می باشد", success = false, orderIds = orderId, shipmentsData = (List<ShipmentsData>)null });
                int _balance = _rewardPointService.GetRewardPointsBalance(_order.CustomerId, 5);
                if (_order.PaymentMethodSystemName == "Payments.CashOnDelivery" || _order.PaymentStatus == PaymentStatus.Paid)
                {
                    _order.OrderStatus = OrderStatus.Complete;
                    _orderService.UpdateOrder(_order);
                }
                else if (_order.PaymentStatus != PaymentStatus.Paid)
                {
                    if (((int)_order.OrderTotal) > _balance)
                    {
                        return Json(new PlaceOrderResult
                        {
                            message = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(_order.OrderTotal).ToString("N0")
                                                                                                             + " ريال باشد. موجودی فعلی " + _balance.ToString("N0")
                            ,
                            success = false,
                            orderId = orderId,
                            shipmentsData = (List<ShipmentsData>)null
                        });
                    }
                    else
                    {
                        int orderTotal = Convert.ToInt32(_order.OrderTotal);
                        _order.OrderTotal = 0;
                        _order.PaymentMethodSystemName = null;
                        _order.PaymentStatus = PaymentStatus.Paid;
                        _orderService.UpdateOrder(_order);
                        _rewardPointService.AddRewardPointsHistoryEntry(_order.Customer, -orderTotal, _order.StoreId, "آزاد شدن برای سفارش شماره" + _order.Id
                            , _order, _order.OrderTotal);
                    }
                }
                int _serviceId = _order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                bool isFreePost = _extendedShipmentService.GetFreePost(_order.OrderItems.First());
                return Json(new PlaceOrderResult { message = "", success = true, orderId = _order.Id, shipmentsData = getShipmentsData(_order.Id, _serviceId, isFreePost, 16) });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new PlaceOrderResult
                {
                    message = "بروز خطا در زمان پرداخت سفارش از کیف پول"
                           ,
                    success = false,
                    orderId = orderId,
                    shipmentsData = (List<ShipmentsData>)null
                });
            }
        }
        [Route("api/CancelOrders")]
        [HttpPost]
        public ActionResult CancelOrders(string orderUniqId)
        {
            int orderId = 0;
            try
            {
                if (string.IsNullOrEmpty(orderUniqId))
                {
                    return Json(new { susccess = false, message = "شماره سفارش مورد نظر نامعتبر می باشد" });
                }
                orderId = _apiOrderRefrenceCodeService.getOrderId(orderUniqId);
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return Json(new { susccess = false, message = "سفارش مورد نظر یافت نشد" });
                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Json(new { susccess = false, message = "این سفارش متعلق به شخص دیگری می باشد و شما امکان کنسل کردن آن را ندارید" });
                if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                    return Json(new { susccess = false, message = "سفارش مورد نظر قبلا کنسل شده" });
                //if (order.Shipments.Any(p => !string.IsNullOrEmpty(p.TrackingNumber)))
                //    return Json(new { susccess = false, message = "به دلیل تولید شدن بارکد پستی امکان ثبت کنسلی این سفارش وجود ندارد، لطفا با پشتبانی هماهنگ کنید" });
                _orderProcessingService.CancelOrder(order, true);
                Nop.plugin.Orders.ExtendedShipment.Services.common.InsertOrderNote("سفارش توسط خود کاربر از طریق api کنسل شده", orderId);
                return Json(new { susccess = true, message = "سفارش شما با موفقیت کنسل شد" });

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { susccess = false, message = "بروز خطا در زمان کنسل کردن سفارش" });

            }
        }
        [Route("api/checkout/PayANdCompleteOrder")]
        [HttpPost]
        public IActionResult PayANdCompleteOrder(string orderUniqId)
        {
            int orderId = 0;
            try
            {
                if (string.IsNullOrEmpty(orderUniqId))
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false,
                        orderId = 0
                        ,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                orderId = _apiOrderRefrenceCodeService.getOrderId(orderUniqId);
                if (orderId <= 0)
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false,
                        orderId = orderId
                        ,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                var _order = _orderService.GetOrderById(orderId);
                if (_order == null)
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false
                        ,
                        orderId = orderId,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                if (_order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Json(new PlaceOrderResult { message = "شماره سفارش وارد شده مطعلق به شخص دیگری می باشد", success = false, orderId = orderId, shipmentsData = (List<ShipmentsData>)null });
                int _balance = _rewardPointService.GetRewardPointsBalance(_order.CustomerId, 5);
                if (_order.PaymentMethodSystemName == "Payments.CashOnDelivery" || _order.PaymentStatus == PaymentStatus.Paid)
                {
                    _extendedShipmentService.CleanToSendDataToPostAgain(new List<int> { _order.Id });
                    _order.OrderStatus = OrderStatus.Complete;
                    _orderService.UpdateOrder(_order);
                }
                else if (_order.PaymentStatus != PaymentStatus.Paid)
                {
                    if (((int)_order.OrderTotal) > _balance)
                    {
                        return Json(new PlaceOrderResult
                        {
                            message = "موجودی کیف پول برای این سفارش می بایست حداقل " + Convert.ToInt32(_order.OrderTotal).ToString("N0")
                                                                                                             + " ريال باشد. موجودی فعلی " + _balance.ToString("N0")
                            ,
                            success = false,
                            orderId = orderId,
                            shipmentsData = (List<ShipmentsData>)null
                        });
                    }
                    else
                    {
                        _extendedShipmentService.CleanToSendDataToPostAgain(new List<int> { _order.Id });
                        int orderTotal = Convert.ToInt32(_order.OrderTotal);
                        _order.OrderTotal = 0;
                        _order.PaymentMethodSystemName = null;
                        _order.PaymentStatus = PaymentStatus.Paid;
                        _orderService.UpdateOrder(_order);
                        _rewardPointService.AddRewardPointsHistoryEntry(_order.Customer, -orderTotal, _order.StoreId, "آزاد شدن برای سفارش شماره" + _order.Id
                            , _order, _order.OrderTotal);
                    }
                }

                int _serviceId = _order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                bool isFreePost = _extendedShipmentService.GetFreePost(_order.OrderItems.First());
                return Json(new PlaceOrderResult { message = "", success = true, orderId = _order.Id, shipmentsData = getShipmentsData(_order.Id, _serviceId, isFreePost, 16) });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new PlaceOrderResult
                {
                    message = "بروز خطا در زمان پرداخت و تکمیل سفارش"
                           ,
                    success = false,
                    orderId = orderId,
                    shipmentsData = (List<ShipmentsData>)null
                });
            }
        }
        private List<ShipmentsData> getShipmentsData(int orderId, int serviceId, bool IsFreePost
            , int SourceId)
        {
            string query = "";
            if (orderId == 0)
                return null;
            var order = _orderService.GetOrderById(orderId);
            int hagheMaghar = _extendedShipmentService.getInsertedHagheMaghar(order);
            int shipmentHaghemaghar = 0;
            if (hagheMaghar > 0)
            {
                shipmentHaghemaghar = hagheMaghar / order.Shipments.Count();
            }
            if (new int[] { 723, 722, 670, 667 }.Contains(serviceId))
            {
                SourceId = SourceId == 0 ? 3 : SourceId;
                query = $@"SELECT
	                S.TrackingNumber,
	                Sa.CodCost gatewayPostPrice,
	                CAST((Sa.CodCost*9/100) AS int) gatewayPostPriceTax,
	                TCCD.EngPrice +((TCCD.EngPrice*9)/100)  PostexEngPrice , 
	                XS.ShipmentTempId,
	                ISNULL(TOIR.SmsPrice,0) + (ISNULL(TOIR.SmsPrice,0)*9/100) SmsPrice,
	                ISNULL(TOIR.AccessPrinterPrice,0) + (ISNULL(TOIR.AccessPrinterPrice,0)*9/100) AccessPrinterPrice,
	                ISNULL(TOIR.InsurancePrice,0) + (ISNULL(TOIR.InsurancePrice,0) *9 /100) InsurancePrice,
	                ISNULL(TOIR.CartonPrice,0) +(ISNULL(TOIR.CartonPrice,0)*9/100) CartonPrice,
	                ISNULL(TOIR.PrintLogoPrice,0) + (ISNULL(TOIR.PrintLogoPrice,0)*9/100) PrintLogoPrice,
	                CAST(ISNULL(TOIR.RegistrationValue,0) AS INT) +(CAST(ISNULL(TOIR.RegistrationValue,0) AS INT)*9/100) RegistrationValue,
	                ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.AgentAddedValue,'0')),0)+(ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.AgentAddedValue,'0')),0)*9/100) AgentAddedValue,
	                ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.GoodsCodPrice,'0')),0)+(ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.GoodsCodPrice,'0')),0)*9/100) GoodsCodPrice,
                    {shipmentHaghemaghar} +({shipmentHaghemaghar}*9/100) shipmentHaghemaghar,
	                TCCD.CodTranPrice,
                    ISNULL(TOIR.DistributionPrice,0)+ (ISNULL(TOIR.DistributionPrice,0) * 9 /100) DistributionPrice
                FROM
	                dbo.Shipment AS S
	                INNER JOIN dbo.ShipmentAppointment AS SA ON SA.ShipmentId = S.Id
	                INNER JOIN dbo.XtnShippment AS XS ON XS.ShipmentId = S.Id
	                INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                INNER JOIN dbo.OrderItem AS OI ON SI.OrderItemId = OI.Id
	                INNER JOIN dbo.Tb_OrderItemsRecord AS TOIR ON TOIR.OrderItemId = OI.Id
	                INNER JOIN dbo.Tb_CodCalculationDetailes AS TCCD ON TCCD.ShipmentId = S.Id
                WHERE
	                S.OrderId ={orderId}";
            }
            else
            {
                query = $@"
                        SELECT
	                        S.TrackingNumber,
	                        S.TrackingNumber,
	                        TCPOI.IncomePrice gatewayPostPrice,
	                        CAST(0 AS INT) gatewayPostPriceTax,
	                        TCPOI.EngPrice PostexEngPrice, 
	                        XS.ShipmentTempId,
	                        ISNULL(TOIR.SmsPrice,0) + (ISNULL(TOIR.SmsPrice,0)*9/100) SmsPrice,
	                        ISNULL(TOIR.AccessPrinterPrice,0) + (ISNULL(TOIR.AccessPrinterPrice,0)*9/100) AccessPrinterPrice,
	                        ISNULL(TOIR.InsurancePrice,0) + (ISNULL(TOIR.InsurancePrice,0) *9 /100) InsurancePrice,
	                        ISNULL(TOIR.CartonPrice,0) +(ISNULL(TOIR.CartonPrice,0)*9/100) CartonPrice,
	                        ISNULL(TOIR.PrintLogoPrice,0) + (ISNULL(TOIR.PrintLogoPrice,0)*9/100) PrintLogoPrice,
	                        CAST(ISNULL(TOIR.RegistrationValue,0) AS INT) +(CAST(ISNULL(TOIR.RegistrationValue,0) AS INT)*9/100) RegistrationValue,
	                        ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.AgentAddedValue,'0')),0)+(ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.AgentAddedValue,'0')),0)*9/100) AgentAddedValue,
	                        ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.GoodsCodPrice,'0')),0)+(ISNULL(dbo.GetOnlyNumbers(ISNULL(TOIR.GoodsCodPrice,'0')),0)*9/100) GoodsCodPrice,
                            {shipmentHaghemaghar} +({shipmentHaghemaghar}*9/100) shipmentHaghemaghar,
                            cast(0 as int) CodTranPrice,
                            ISNULL(TOIR.DistributionPrice,0)+ (ISNULL(TOIR.DistributionPrice,0) * 9 /100) DistributionPrice
                        FROM
	                        dbo.[Order] AS O
	                        INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                        INNER JOIN dbo.XtnShippment AS XS ON XS.ShipmentId = S.Id
	                        INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                        INNER JOIN dbo.OrderItem AS OI ON SI.OrderItemId = OI.Id
	                        INNER JOIN dbo.Tb_CalcPriceOrderItem AS TCPOI ON TCPOI.OrderItemId = OI.Id
	                        INNER JOIN dbo.Tb_OrderItemsRecord AS TOIR ON TOIR.OrderItemId = OI.Id
                        WHERE
	                        O.Id = {orderId}";
            }
            var data = _dbContext.SqlQuery<ShipmentsData>(query, new object[0]).ToList();
            return data;
        }
        public class ShipmentsData
        {
            public string TrackingNumber { get; set; }
            public int gatewayPostPrice { get; set; }
            public int gatewayPostPriceTax { get; set; }
            public int PostexEngPrice { get; set; }
            public int? ShipmentTempId { get; set; }
            public int SmsPrice { get; set; }
            public int AccessPrinterPrice { get; set; }
            public int InsurancePrice { get; set; }
            public int CartonPrice { get; set; }
            public int PrintLogoPrice { get; set; }
            public int RegistrationValue { get; set; }
            public int AgentAddedValue { get; set; }
            public int GoodsCodPrice { get; set; }
            public int shipmentHaghemaghar { get; set; }
            public int CodTranPrice { get; set; }
            public int DistributionPrice { get; set; }

        }
        private IActionResult _GhasedakCheckoutOrder(CheckoutItemApi model)
        {
            lock ("customer_" + _workContext.CurrentCustomer.Id)
            {
                if (_storeContext.CurrentStore.Id == 3)
                {
                    return Json(new { resultCode = 40, message = "سرویس مورد نظر غیر فعال شده. لطفا جهت اطلاعات بیشتر با واحد پشتیبانی تماس بگیرید" }); ;
                }
                if (_workContext.CurrentCustomer.Id != 4144899 && _workContext.CurrentCustomer.Id != 11295490)
                {
                    if (model.SenderLat == null || model.SenderLon == null)
                    {
                        return Json(new { resultCode = 21, message = ApiMessage.GetErrorMsg(21) });
                    }
                    if (((!model.width.HasValue || !model.height.HasValue || !model.length.HasValue) ||
                            (model.height.Value == 0 || model.width.Value == 0 || model.length.Value == 0))
                        && (string.IsNullOrEmpty(model.CartonSizeName) || model.CartonSizeName == "کارتن نیاز ندارم."))
                    {
                        model.CartonSizeName = "سایز 4(20*20*30)";
                    }
                }
                if (model.CartonSizeName == "سایز A4(31*22)")
                    model.Weight = 2000;
                else if (model.CartonSizeName == "سایز A3(30*45)")
                    model.Weight = 4900;
                else if (model.CartonSizeName == "سایز 3(15*20*20)")
                    model.Weight = 7000;
                else
                {
                    return Json(new { resultCode = -1, message = "سایز بسته وارد شده برای شما نا معتبر می با شد" });
                }

                var Dimention = _newCheckout.getDimentionByName(model.CartonSizeName);
                if (Dimention != null)
                {

                    model.width = Dimention.Width;
                    model.height = (Dimention.Height == 0 ? 2 : Dimention.Height);
                    model.length = Dimention.Length;
                }


                if (!IsValidCustomer())
                    return Json(new { resultCode = 16, message = ApiMessage.GetErrorMsg(16) });
                if (model == null)
                    return Json(new { resultCode = 1, message = ApiMessage.GetErrorMsg(1) });
                if (_extendedShipmentService.isInvalidSender(model.Sender_StateId, model.Sender_townId))
                {
                    return Json(new { resultCode = 18, message = ApiMessage.GetErrorMsg(18) });
                }
                string error = "";
                int resultCode = model.IsValidGhasedak(out error);

                if (resultCode != 0)
                {
                    //Todo  add Log
                    return Json(new { resultCode = resultCode, message = error });
                }


                if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                {
                    lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                    {
                        if (!_apiOrderRefrenceCodeService.CheckAndInsertApiOrderRefrenceCode(_workContext.CurrentCustomer.Id, model.refrenceNo, out Tbl_ApiOrderRefrenceCode tbl_ApiOrderRefrenceCode))
                        {
                            return Json(new { resultCode = 19, message = string.Format(ApiMessage.GetErrorMsg(19), model.refrenceNo, tbl_ApiOrderRefrenceCode.OrderId.Value), orderId = tbl_ApiOrderRefrenceCode.OrderId.Value });
                        }
                    }
                }

                #region CreateSpModel
                NewCheckoutModel ApiCheckoutModel = new NewCheckoutModel()
                {
                    AgentSaleAmount = model.AgentSaleAmount,
                    boxType = (model.boxType == "0" ? "پاکت" : "بسته"),
                    CartonSizeName = model.CartonSizeName,
                    CodGoodsPrice = model.CodGoodsPrice,
                    Count = model.Count,
                    discountCouponCode = model.discountCouponCode,
                    dispatch_date = model.dispatch_date,
                    //getItNow 
                    GoodsType = model.GoodsType,
                    HasAccessToPrinter = model.HasAccessToPrinter,
                    hasNotifRequest = model.notifBySms,
                    height = model.height,
                    width = model.width,
                    length = model.length,
                    InsuranceName = model.InsuranceName,
                    IsCOD = model.IsCOD,
                    receiver_ForeginCityName = model.receiver_ForeginCityName,
                    receiver_ForeginCountry = model.receiver_ForeginCountry.GetValueOrDefault(0),
                    receiver_ForeginCountryName = model.receiver_ForeginCountryName,
                    receiver_ForeginCountryNameEn = model.receiver_ForeginCountryName,
                    ApproximateValue = model.ApproximateValue,
                    RequestPrintAvatar = model.printLogo,
                    ReciverLat = model.ReciverLat,
                    ReciverLon = model.ReciverLon,
                    SenderLat = model.SenderLat,
                    SenderLon = model.SenderLon,
                    UbbarPackingLoad = model.UbbarPackingLoad,
                    ServiceId = model.ServiceId,
                    _dispatch_date = model._dispatch_date,
                    Weight = model.Weight,
                    VechileOptions = model.VechileOptions,
                    UbbraTruckType = model.UbbraTruckType,
                    needCaton = model.NeedCarton
                };
                ApiCheckoutModel.billingAddressModel = new Address()
                {
                    Address1 = model.Sender_Address,
                    City = model.Sender_City,
                    CountryId = model.Sender_StateId,
                    StateProvinceId = model.Sender_townId,
                    Email = model.Sender_Email,
                    FirstName = model.Sender_FristName,
                    LastName = model.Sender_LastName,
                    PhoneNumber = model.Sender_mobile,
                    ZipPostalCode = model.Sender_PostCode
                };
                ApiCheckoutModel.shippingAddressModel = new Address()
                {
                    Address1 = model.Reciver_Address,
                    City = model.Reciver_City,
                    CountryId = model.Reciver_StateId,
                    StateProvinceId = model.Reciver_townId,
                    Email = model.Reciver_Email,
                    FirstName = model.Reciver_FristName,
                    LastName = model.Reciver_LastName,
                    PhoneNumber = model.Reciver_mobile,
                    ZipPostalCode = model.Reciver_PostCode
                };

                #endregion

                string resultMessage = "";
                List<string> strError = new List<string>();

                var inputModel = new NewCheckout_Sp_Input()
                {
                    JsonOrderList = JsonConvert.SerializeObject(new List<NewCheckoutModel>() { ApiCheckoutModel }),
                    JsonData = JsonConvert.SerializeObject(new { CustommerId = _workContext.CurrentCustomer.Id, CustommerIp = _webHelper.GetCurrentIpAddress(), StoreId = _storeContext.CurrentStore.Id, IsWebApi = true })
                };
                try
                {
                    var ret = _newCheckout.CheckoutBySp(inputModel, OrderRegistrationMethod.Api, 0, false, false);
                    if (ret.orderId > 0)
                    {
                        var order = _orderService.GetOrderById(ret.orderId);
                        _reOrderService.InsertOrderJson(order.Id, JsonConvert.SerializeObject(model), true);
                        if (_workContext.CurrentCustomer.AffiliateId == 1149)
                        {
                            _extendedShipmentService._GenerateBarcodes(order, out strError);
                            if (strError.Count() > 0)
                            {
                                resultMessage = ApiMessage.GetErrorMsg(17) + "\r\n" + string.Join("\r\n", strError);
                                return Json(new { resultCode = 17, message = resultMessage, orderId = order.Id });
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(model.refrenceNo))
                        {
                            lock (model.refrenceNo + "_" + _workContext.CurrentCustomer.Id)
                            {
                                _apiOrderRefrenceCodeService.SetOrderId(_workContext.CurrentCustomer.Id, model.refrenceNo, ret.orderId);
                            }
                        }
                        if (!IsOkOrder(order.Id) && _orderProcessingService.CanCancelOrder(order))
                        {
                            _orderProcessingService.CancelOrder(order, false);
                            resultMessage = ApiMessage.GetErrorMsg(22) + "\r\n" + string.Join("\r\n", strError);
                            return Json(new { resultCode = 22, message = resultMessage, orderId = order.Id });
                        }
                        resultMessage = ApiMessage.GetErrorMsg(0) + "\r\n" + string.Join("\r\n", strError);
                        int orderTotal = 0;
                        if (order.OrderTotal == 0 && order.PaymentMethodSystemName == null)
                        {
                            if (order.RedeemedRewardPointsEntry != null)
                            {
                                orderTotal = order.RedeemedRewardPointsEntry.Points * -1;
                            }
                        }
                        return Json(new { resultCode = 0, message = resultMessage, orderId = order.Id, orderTotal = Convert.ToInt32(orderTotal) });
                    }
                    return Json(new { message = ret.ErrorMessage, success = ret.ErrorCode == 0, orderIds = ret.orderId });
                }
                catch (Exception ex)
                {
                    return Json(new { message = ex.Message, success = false });
                }

            }

        }
        [Route("api/CancelOrder")]
        [HttpGet]
        public ActionResult CancelOrder(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return Json(new { susccess = false, message = "سفارش مورد نظر یافت نشد" });
                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Json(new { susccess = false, message = "این سفارش متعلق به شخص دیگری می باشد و شما امکان کنسل کردن آن را ندارید" });
                if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                    return Json(new { susccess = false, message = "سفارش مورد نظر قبلا کنسل شده" });
                //if (order.Shipments.Any(p => !string.IsNullOrEmpty(p.TrackingNumber)))
                //    return Json(new { susccess = false, message = "به دلیل تولید شدن بارکد پستی امکان ثبت کنسلی این سفارش وجود ندارد، لطفا با پشتبانی هماهنگ کنید" });
                _orderProcessingService.CancelOrder(order, true);
                Nop.plugin.Orders.ExtendedShipment.Services.common.InsertOrderNote("سفارش توسط خود کاربر از طریق api کنسل شده", orderId);
                return Json(new { susccess = true, message = "سفارش شما با موفقیت کنسل شد" });

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { susccess = false, message = "بروز خطا در زمان کنسل کردن سفارش" });

            }
        }
        [Route("api/checkout/SetPay")]
        public IActionResult Setpayment([FromBody] SetPayModel model)
        {
            lock ("customer_" + _workContext.CurrentCustomer.Id)
            {
                #region Validation
                if (!_workContext.CurrentCustomer.IsInCustomerRole("PaymentAssistance"))
                {
                    return Json(new { resultCode = 100, message = "امکان استفاده شما از این سرویس وجود ندارد" });
                }
                if (string.IsNullOrEmpty(model.AuthorizationTransactionId))
                {
                    return Json(new { resultCode = 101, message = "کد تراکنش بانک نامعتبر می باشد" });
                }
                if (model.OrderId <= 0)
                {
                    return Json(new { resultCode = 102, message = "شناسه سفارش نامعتبر می باشد" });
                }
                if (model.PayDate == null)
                {
                    return Json(new { resultCode = 103, message = "تاریخ پرداخت نامعتبر می باشد" });
                }
                var order = _orderService.GetOrderById(model.OrderId);
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    return Json(new { resultCode = 104, message = "سفارش مورد نظر کنسل شده و امکان پرداخت آن وجود ندارد" });
                }
                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                {
                    return Json(new { resultCode = 105, message = "امکان ثبت پرداخت این سفارش برای شما وجود ندارد" });
                }
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    return Json(new { resultCode = 101, message = "این سفارش قبلا پرداخت شده" });
                }
                #endregion

                order.AuthorizationTransactionId = model.AuthorizationTransactionId;
                order.PaidDateUtc = model.PayDate.ToUniversalTime();
                _orderService.UpdateOrder(order);
                _orderProcessingService.MarkOrderAsPaid(order);
                return Json(new { resultCode = 0, message = "عملیات با موفقیت انجام شد" });
            }
        }
        [Route("api/checkout/FinalizeOrder")]
        [HttpPost]
        public IActionResult SendDataToPost(int orderId)
        {

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return Json(new { resultCode = 13, message = ApiMessage.GetErrorMsg(13) });
            }
            if (!order.Customer.IsInCustomerRole("TwoStepOrder"))
            {
                return Json(new { resultCode = 15, message = ApiMessage.GetErrorMsg(15) });
            }
            string strError = "";
            var result = _bulkOrderService._SendDataToPost(order, out strError);
            if (!result)
            {
                return Json(new { resultCode = 14, message = strError });
            }
            return Json(new { resultCode = 0, message = ApiMessage.GetErrorMsg(0) });

        }

        [Route("api/checkout/ChangeOrderStatus")]
        [HttpPost]
        public IActionResult ChangeOrderStatus([FromBody] string TrackingNumber, [FromBody] string UniqueReferenceNo)
        {
            if (string.IsNullOrEmpty(TrackingNumber) && string.IsNullOrEmpty(UniqueReferenceNo))
            {
                return Json(new
                {
                    success = false,
                    message = $"Invaild Input Error!"
                });
            }
            int gatewayState = 2;
            if (!string.IsNullOrEmpty(TrackingNumber))
            {
                var bres = _codService.ChangeStatus(gatewayState, TrackingNumber, out string res);
                if (!bres) return Json(new { success = false, message = $"Error While Change State, Tracking Number = {TrackingNumber + Environment.NewLine + res}" });
            }
            else
            {
                var TrackingNumbersList = _codService.GetTrackingNumbersByUniqueReferenceNo(UniqueReferenceNo);
                foreach (string item in TrackingNumbersList)
                {
                    var bres = _codService.ChangeStatus(gatewayState, item, out string res);
                    if (!bres) return Json(new { success = false, message = $"Error While Change State, Tracking Number = {item + Environment.NewLine + res}" });
                }
            }
            return Json(new
            {
                success = true,
                message = $"Status Changed Successfully :)"
            });
        }
        #endregion

        #region Utility
        [NonAction]
        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        #endregion
        private bool IsOkOrder(int OrderId)
        {
            string Query = $@"SELECT
	                TOP(1) cast(1 as bit) IsOk
                FROM
	                dbo.[Order] AS O
	                INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                INNER JOIN dbo.OrderItem AS OI ON OI.Id = SI.OrderItemId
	                INNER JOIN dbo.XtnShippment AS XS ON XS.ShipmentId = S.Id
                WHERE
	                o.id = {OrderId}";
            return _dbContext.SqlQuery<bool?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(false);

        }
        [Route("api/GetChargeWalletHistory")]
        [HttpPost]
        public IActionResult GetChargeWalletHistory([FromBody] chargeWalletFilterModel model)
        {
            try
            {
                string query = $@"EXEC dbo.Sp_ChargeWalletHistoryForPanel @CustomerUserName , @TrackingNumber,@OrderId,@StartDate, @EndDate, @PageIndex, @PageSize, @AllRowCount OUTPUT";

                SqlParameter P_CustomerUserName = new SqlParameter()
                {
                    ParameterName = "CustomerUserName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Value = (object)_workContext.CurrentCustomer.Username
                };
                SqlParameter P_TrackingNumber = new SqlParameter()
                {
                    ParameterName = "TrackingNumber",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 25,
                    Value = (string.IsNullOrEmpty(model.TrackingNumber) ? (object)DBNull.Value : (object)model.TrackingNumber)
                };
                SqlParameter P_OrderId = new SqlParameter()
                {
                    ParameterName = "OrderId",
                    SqlDbType = SqlDbType.VarChar,
                    Value = (!model.OrderId.HasValue ? (object)DBNull.Value : (object)model.OrderId)
                };
                SqlParameter P_StartDate = new SqlParameter()
                {
                    ParameterName = "StartDate",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (!model.StartDate.HasValue ? (object)DBNull.Value : (object)model.StartDate)
                };
                SqlParameter P_EndDate = new SqlParameter()
                {
                    ParameterName = "EndDate",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (!model.EndDate.HasValue ? (object)DBNull.Value : (object)model.EndDate)
                };
                SqlParameter P_PageIndex = new SqlParameter()
                {
                    ParameterName = "PageIndex",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)model.PageIndex
                };
                SqlParameter P_PageSize = new SqlParameter()
                {
                    ParameterName = "PageSize",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)model.PageSize
                };
                SqlParameter P_AllRowCount = new SqlParameter()
                {
                    ParameterName = "AllRowCount",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                    Value = (object)0
                };
                SqlParameter[] prms = new SqlParameter[]{
                P_CustomerUserName,
                P_TrackingNumber,
                P_OrderId,
                P_StartDate,
                P_EndDate,
                P_PageIndex,
                P_PageSize,
                P_AllRowCount
            };
                var data = _dbContext.SqlQuery<ChageWalletModel>(query, prms).ToList();
                return Json(new { AllRowCount = P_AllRowCount.Value.ToString(), data = data, message = "", success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { AllRowCount = 0, data = (List<ChageWalletModel>)null, message = "", success = false });
            }
        }

        [Route("api/GetMyChargeWalletHistory")]
        [HttpPost]
        public IActionResult GetMyChargeWalletHistory([FromBody] chargeWalletFilterModel model)
        {
            try
            {
                var customer = _customerService.GetCustomerByUsername(model.UserName);
                if (customer == null)
                {
                    return Json(new { AllRowCount = 0, data = (List<ChageWalletModel>)null, message = "نام کاربری وارد شده نامعتبر می باشد", success = false });
                }
                if (customer.Id != _workContext.OriginalCustomerIfImpersonated.Id)
                {
                    return Json(new { AllRowCount = 0, data = (List<ChageWalletModel>)null, message = "نام کاربری متعلق به شما نمی باشد", success = false });
                }
                if (model.PageSize > 500)
                {
                    return Json(new { AllRowCount = 0, data = (List<ChageWalletModel>)null, message = "تعداد رکورد در هر صفحه نمی تواند بیشتر از 500 رکورد باشدس", success = false });
                }
                string query = $@"EXEC dbo.Sp_ChargeWalletHistoryForPanel @CustomerUserName , @TrackingNumber,@OrderId,@StartDate, @EndDate, @PageIndex, @PageSize, @AllRowCount OUTPUT";

                SqlParameter P_CustomerUserName = new SqlParameter()
                {
                    ParameterName = "CustomerUserName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Value = (object)model.UserName
                };
                SqlParameter P_TrackingNumber = new SqlParameter()
                {
                    ParameterName = "TrackingNumber",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 25,
                    Value = (string.IsNullOrEmpty(model.TrackingNumber) ? (object)DBNull.Value : (object)model.TrackingNumber)
                };
                SqlParameter P_OrderId = new SqlParameter()
                {
                    ParameterName = "OrderId",
                    SqlDbType = SqlDbType.VarChar,
                    Value = (!model.OrderId.HasValue ? (object)DBNull.Value : (object)model.OrderId)
                };
                SqlParameter P_StartDate = new SqlParameter()
                {
                    ParameterName = "StartDate",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (!model.StartDate.HasValue ? (object)DBNull.Value : (object)model.StartDate)
                };
                SqlParameter P_EndDate = new SqlParameter()
                {
                    ParameterName = "EndDate",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (!model.EndDate.HasValue ? (object)DBNull.Value : (object)model.EndDate)
                };
                SqlParameter P_PageIndex = new SqlParameter()
                {
                    ParameterName = "PageIndex",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)model.PageIndex
                };
                SqlParameter P_PageSize = new SqlParameter()
                {
                    ParameterName = "PageSize",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)model.PageSize
                };
                SqlParameter P_AllRowCount = new SqlParameter()
                {
                    ParameterName = "AllRowCount",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                    Value = (object)0
                };
                SqlParameter[] prms = new SqlParameter[]{
                P_CustomerUserName,
                P_TrackingNumber,
                P_OrderId,
                P_StartDate,
                P_EndDate,
                P_PageIndex,
                P_PageSize,
                P_AllRowCount
            };
                var data = _dbContext.SqlQuery<ChageWalletModel>(query, prms).ToList();
                return Json(new { AllRowCount = P_AllRowCount.Value.ToString(), data = data, message = "", success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { AllRowCount = 0, data = (List<ChageWalletModel>)null, message = "", success = false });
            }
        }

        [Route("api/getWalletTransaction")]
        [HttpPost]
        public IActionResult getWalletTransaction([FromBody] WalletTransactionFilterModel model)
        {
            try
            {
                string Message = "";
                if (model.PageIndex < 0)
                    Message = "مقدار وارد شده برای شماره صفحه اطلاعات نامعتبر میباشد";

                if (model.PageSize <= 0 || model.PageSize > 1000)
                    Message = "مقدار وارد شده برای اندازه صفحه اطلاعات باید عددی بین 1 تا 1000 باشد";

                if (string.IsNullOrEmpty(model.UserName))
                {
                    Message = "نام کاربری نا معتبر می باشد";
                }
                if (Message != "")
                    return Json(new { AllRowCount = 0, data = (List<RewardPointsHistory>)null, message = Message, success = false });
                Customer _Customer = null;
                if (model.IsOrganisation)
                    _Customer = _customerService.GetCustomerByUsername(model.UserName);
                else
                    _Customer = _workContext.CurrentCustomer;

                if (_Customer == null)
                    return Json(new { AllRowCount = 0, data = (List<RewardPointsHistory>)null, message = "نام کاربری نا معتبر می باشد", success = false });
                SqlParameter P_AllRowCount = new SqlParameter()
                {
                    ParameterName = "TotalCount",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                    Value = (object)0
                };
                SqlParameter[] prms = new SqlParameter[] { P_AllRowCount };
                string query = $@"
                            SELECT
	                             @TotalCount = COUNT(1)
                            FROM
	                            dbo.RewardPointsHistory AS RPH
	                            INNER JOIN dbo.Customer AS C ON C.Id = RPH.CustomerId
                            WHERE
	                            C.Username ='{model.UserName}'
                            SELECT
	                             RPH.Points
	                            , RPH.PointsBalance
	                            , RPH.Message
	                            , dbo.GregorianToJalali(dbo.Fn_ConvertToLocalDate(RPH.CreatedOnUtc),'yyyy/MM/dd HH:mm:ss') CreatedOn
                            FROM
	                            dbo.RewardPointsHistory AS RPH
	                            INNER JOIN dbo.Customer AS C ON C.Id = RPH.CustomerId
                            WHERE
	                            C.Username ='{model.UserName}'
                            ORDER BY RPH.CreatedOnUtc
                            OFFSET 
	                            {model.PageSize} * ({model.PageIndex}) ROWS FETCH NEXT {model.PageSize} ROWS ONLY;";
                var result = _dbContext.SqlQuery<WalletTransactionModel>(query, prms).ToList();
                return Json(new { AllRowCount = P_AllRowCount.Value.ToString(), data = result.ToList(), message = "", success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { AllRowCount = 0, data = (List<RewardPointsHistory>)null, message = "بروز خطا در زمان واکشی اطلاعات", success = false });
            }
        }

        [Route("api/getCashoutDetailes")]
        [HttpPost]
        public IActionResult getCashoutDetailes(int payCode)
        {
            try
            {
                string query = $@"SELECT
	                                RPh.Points,
	                                RPH.Message,
	                                TCWT.ChargeWalletTypeName,
	                                S.TrackingNumber,
	                                TAORC.RefrenceCode
                                FROM
	                                dbo.Tb_RPC_ExportHistoryCustomer AS TREHC
	                                INNER JOIN dbo.Tb_RPC_ExportHistoryDetails AS TREHD ON TREHD.xRPCExport_Id = TREHC.Id
	                                INNER JOIN dbo.[Order] AS O ON O.Id= TREHD.xRPC_Id
	                                INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                                INNER JOIN dbo.Tb_ChargeWalletHistory AS TCWH ON TCWH.shipmentId = S.Id
	                                INNER JOIN dbo.RewardPointsHistory AS RPH ON RPH.Id = TCWH.rewaridPointHistoryId
	                                INNER JOIN dbo.Tb_ChargeWalletType AS TCWT ON TCWT.Id = TCWH.ChargeWalletTypeId
	                                LEFT JOIN dbo.Tbl_ApiOrderRefrenceCode AS TAORC ON TAORC.OrderId = O.Id
                                WHERE
	                                TREHC.xRPCExport_Id = {payCode}";


                var data = _dbContext.SqlQuery<getCashoutDetailesOutputModel>(query, new object[0]).ToList();
                return Json(new { data = data, message = "", success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { data = (List<getCashoutDetailesOutputModel>)null, message = "خطا در زمان دریافت اطلاعات", success = false });
            }
        }

        [Route("api/getCashoutMaster")]
        [HttpPost]
        public IActionResult getCashoutmaster()
        {
            try
            {

                string query = $@"SELECT
	                                TREH.Id PayCode
	                                , CAST(TREH.xCreatedDate AS DATE) PayDate
	                                , RPH.Points* -1 PayValue
                                FROM
	                                dbo.Tb_RPC_ExportHistory AS TREH
	                                INNER JOIN dbo.Tb_RPC_ExportHistoryCustomer AS TREHC ON TREHC.xRPCExport_Id= TREH.Id
	                                INNER JOIN dbo.RewardPointsHistory AS RPH ON Rph.Id = TREHC.RewardPointHistoryId
	                                INNER JOIN dbo.Customer AS C ON TREHC.CustomerId = C.Id 
                                WHERE
	                                TREH.xIsDeleted = 0
	                                AND C.Username = '{_workContext.CurrentCustomer.Username}'
                                ORDER BY TREH.Id DESC";


                var data = _dbContext.SqlQuery<getCashoutMasterOutputModel>(query, new object[0]).ToList();
                return Json(new { data = data, message = "", success = true });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { data = (List<getCashoutMasterOutputModel>)null, message = "خطا در زمان دریافت اطلاعات", success = false });
            }
        }
        [Route("api/getOrderFinancialData")]
        [HttpPost]
        public IActionResult getOrderFinancial(string orderUniqId)
        {
            int orderId = 0;
            try
            {
                if (string.IsNullOrEmpty(orderUniqId))
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false,
                        orderId = 0
                        ,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                orderId = _apiOrderRefrenceCodeService.getOrderId(orderUniqId);
                if (orderId <= 0)
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false,
                        orderId = orderId
                        ,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                var _order = _orderService.GetOrderById(orderId);
                if (_order == null)
                    return Json(new PlaceOrderResult
                    {
                        message = "شماره سفارش وارد شده نامعتبر می باشد",
                        success = false
                        ,
                        orderId = orderId,
                        shipmentsData = (List<ShipmentsData>)null,
                        resultCode = 13
                    });
                if (_order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Json(new PlaceOrderResult { message = "شماره سفارش وارد شده مطعلق به شخص دیگری می باشد", success = false, orderId = orderId, shipmentsData = (List<ShipmentsData>)null });
                int _serviceId = _order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                bool isFreePost = _extendedShipmentService.GetFreePost(_order.OrderItems.First());
                return Json(new PlaceOrderResult { message = "", success = true, orderId = _order.Id, shipmentsData = getShipmentsData(_order.Id, _serviceId, isFreePost, 16) });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new PlaceOrderResult
                {
                    message = "خطا در زمان دریافت اطلاعات مالی سفارش",
                    success = false,
                    orderId = orderId,
                    shipmentsData = (List<ShipmentsData>)null
                });
            }
        }
    }
    public class getCashoutMasterOutputModel
    {
        public long PayCode { get; set; }
        public DateTime? PayDate { get; set; }
        public int PayValue { get; set; }
    }
    public class getCashoutDetailesInputModel
    {
        public string UserName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class getCashoutDetailesOutputModel
    {
        public int Points { get; set; }
        public string Message { get; set; }
        public string ChargeWalletTypeName { get; set; }
        public string TrackingNumber { get; set; }
        public string RefrenceCode { get; set; }
    }
    public class chargeWalletFilterModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int? OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string UserName { get; set; }
    }
    public class WalletTransactionFilterModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string UserName { get; set; }
        public bool IsOrganisation { get; set; }
    }
    public class WalletTransactionModel
    {
        public int Points { get; set; }
        public int PointsBalance { get; set; }
        public string Message { get; set; }
        public string CreatedOn { get; set; }
    }
    public class ChageWalletModel
    {
        public string PayDate { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public int Dicsount { get; set; }
        public int SmsPrice { get; set; }
        public int PrintLogoPrice { get; set; }
        public int CartonPrice { get; set; }
        public int AccessPrinterPrice { get; set; }
        public int InsurancePrice { get; set; }
        public int RegistrationValue { get; set; }
        public int Point { get; set; }
        public string Description { get; set; }
        public string ChargeWalletTypeName { get; set; }
        public int GoodsCodPrice { get; set; }
        public int FactorPrice { get; set; }
    }
}
