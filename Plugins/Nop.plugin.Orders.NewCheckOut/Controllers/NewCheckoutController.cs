using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Core.Plugins;
using Nop.plugin.Orders.NewCheckOut.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.plugin.Orders.NewCheckOut.Controllers
{
    [HttpsRequirement(SslRequirement.Yes)]
    public partial class NewCheckoutController : BasePluginController
    {
        #region Fields

        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IStoreService _storeService;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public NewCheckoutController(
            ICheckoutModelFactory checkoutModelFactory,
            IWorkContext workContext,
            IStoreContext storeContext,
            IShoppingCartService shoppingCartService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IOrderProcessingService orderProcessingService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IPaymentService paymentService,
            IPluginFinder pluginFinder,
            ILogger logger,
            IOrderService orderService,
            IWebHelper webHelper,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IStoreService storeService,
            ISettingService settingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter
            )
        {
            this._checkoutModelFactory = checkoutModelFactory;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._productService = productService;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._storeService = storeService;
            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._customerSettings = customerSettings;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
        }

        #endregion

        #region Utilities

        protected bool IsMinimumOrderPlacementIntervalValid(Customer customer)
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

        #region Methods (common)
        public IActionResult Index()
        {
            if (!IsUserInRole())
                RedirectToRoute("Checkout");
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");


            var downloadableProductsRequireRegistration =
                _customerSettings.RequireRegistrationForDownloadableProducts && cart.Any(sci => sci.Product.IsDownload);

            if (_workContext.CurrentCustomer.IsGuest() && (!_orderSettings.AnonymousCheckoutAllowed || downloadableProductsRequireRegistration))
                return Challenge();

            //if we have only "button" payment methods available (displayed on the shopping cart page, not during checkout),
            //then we should allow standard checkout
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            if (!nonButtonPaymentMethods.Any() && buttonPaymentMethods.Any())
                return RedirectToRoute("ShoppingCart");

            //reset checkout data
            _customerService.ResetCheckoutData(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            //validation (cart)
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            var scWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, true);
            if (scWarnings.Any())
                return RedirectToRoute("ShoppingCart");
            //validation (each shopping cart item)
            foreach (var sci in cart)
            {
                var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(_workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                if (sciWarnings.Any())
                    return RedirectToRoute("ShoppingCart");
            }

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            return RedirectToRoute("CheckoutBillingAddress");
        }

        public IActionResult Completed(int? orderId)
        {
            //validation
            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = _orderService.GetOrderById(orderId.Value);
            }
            if (order == null)
            {
                order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return RedirectToRoute("HomePage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //model
            var model = _checkoutModelFactory.PrepareCheckoutCompletedModel(order);
            return View(model);
        }

        #endregion

        #region Methods (one page checkout)

        public IActionResult OrderValueDividision()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();
            if (!cart.Any())
                throw new Exception("Your cart is empty");

            if (!_orderSettings.OnePageCheckoutEnabled)
                throw new Exception("One page checkout is disabled");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                throw new Exception("Anonymous checkout is not allowed");

            //prevent 2 orders being placed within an X seconds time frame
            if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));
            return PartialView("/Plugins/Orders.NewCheckOut/Views/DivisionTotalValue.cshtml", getDivisionModel(cart));
        }

        public IActionResult OnePageCheckout()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            var model = _checkoutModelFactory.PrepareOnePageCheckoutModel(cart);
            return View("/Plugins/Orders.NewCheckOut/Views/NewCheckOut.cshtml", model);
        }


        public IActionResult SaveDivision(OrderDivisionModel model)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");
            decimal remining = 0;
            if (!CheckDivisionIsOk(cart, model, out remining))
            {
                return Json(new { isOk = false, reminingPrice = remining, message = "لطفا مقادیر وارد شده بررسی کنید. مبلغ باقیمانده باید به صفر برسد" });
            }
            string Msg = "";
           bool isOk= OpcConfirmOrder(model,out Msg);
            return Json(new { isOk = isOk, reminingPrice = remining, message = Msg });
        }
        public IActionResult CheckDivisionIsOk(OrderDivisionModel model)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();
            decimal remining = 0;
            bool IsOk= CheckDivisionIsOk(cart, model, out remining);
            return Json(new { isOk = IsOk, reminingPrice = remining });
        }
        private bool CheckDivisionIsOk(List<ShoppingCartItem> cart, OrderDivisionModel model,out decimal remining) {
            var BaseData = getDivisionModel(cart);
            remining = BaseData.TotalPrice - (model.AmountOfCash
                            + (model.AmountOfDaller * BaseData.BaseDallerPrice)
                            + (model.AmountOfGold * BaseData.BaseGoldPriceInGram)
                       );
            if (remining == 0)
                return true;
            
            return false;
        }


       

        private bool CheckoutSaveAddressId2()
        {
            //int addressId;
            //int.TryParse(value.Value, out addressId);
            //GeneralResponseModel<bool> generalResponseModel = new GeneralResponseModel<bool>();
            //bool flag = addressId > 0;
            //if (flag)
            {
                Address address = this._workContext.CurrentCustomer.Addresses.FirstOrDefault();
                bool flag2 = address == null;
                if (flag2)
                {
                    return false;
                }
                //bool flag3 = addressType == 1;
                //if (flag3)
                {
                    this._workContext.CurrentCustomer.BillingAddress = address;
                }
                //else
                {
                    //bool flag4 = addressType == 2;
                    //if (flag4)
                    {
                        this._workContext.CurrentCustomer.ShippingAddress = address;
                        this._genericAttributeService.SaveAttribute<PickupPoint>(this._workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, this._storeContext.CurrentStore.Id);
                    }
                }
                this._customerService.UpdateCustomer(this._workContext.CurrentCustomer);
                return true;
                //generalResponseModel.Data = true;
            }
            //else
            //{
            //    generalResponseModel.StatusCode = 400;
            //    generalResponseModel.Data = false;
            //    generalResponseModel.ErrorList = new List<string>
            //    {
            //        "Address can't be loaded"
            //    };
            //}
            //return this.Ok(generalResponseModel);
        }



        private bool CheckoutSetShippingMethods2()
        {
            //GeneralResponseModel<bool> generalResponseModel = new GeneralResponseModel<bool>();
            List<ShoppingCartItem> cart = (from sci in this._workContext.CurrentCustomer.ShoppingCartItems
                                           where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                                           select sci).LimitPerStore(this._storeContext.CurrentStore.Id).ToList<ShoppingCartItem>();
            //string value2 = value.Value;
            //bool flag = string.IsNullOrEmpty(value2);
            //if (flag)
            //{
            //    throw new Exception("Selected shipping method can't be parsed");
            //}
            //string[] array = value2.Split(new string[]
            //{
            //    "___"
            //}, StringSplitOptions.RemoveEmptyEntries);
            //bool flag2 = array.Length != 2;
            //if (flag2)
            //{
            //    throw new Exception("Selected shipping method can't be parsed");
            //}
            //string selectedName = array[0];
            //string shippingRateComputationMethodSystemName = array[1];
            var value = this._checkoutModelFactory.PrepareShippingMethodModel(cart, this._workContext.CurrentCustomer.ShippingAddress);
            _logger.Warning("MM-0 " + this._storeContext.CurrentStore.Id);
            List<ShippingOption> list = this._workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(SystemCustomerAttributeNames.OfferedShippingOptions, this._storeContext.CurrentStore.Id);
            _logger.Warning("MM0 " + list.Count);
            //bool flag3 = list == null || list.Count == 0;
            //if (flag3)
            //{
            //    list = this._shippingService.GetShippingOptions(cart, this._workContext.CurrentCustomer.ShippingAddress, this._workContext.CurrentCustomer, shippingRateComputationMethodSystemName, this._storeContext.CurrentStore.Id).ShippingOptions.ToList<ShippingOption>();
            //}
            //else
            {
                list = (from so in list
                        //where so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase)
                        select so).ToList<ShippingOption>();
            }
            _logger.Warning("MM1 ");
            ShippingOption shippingOption = list.Find((ShippingOption so) => !string.IsNullOrEmpty(so.Name));// && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            bool flag4 = shippingOption == null;
            if (flag4)
            {
                return false;
                //throw new Exception("Selected shipping method can't be loaded");
            }
            _logger.Warning("MM2 " + (shippingOption==null));
            this._genericAttributeService.SaveAttribute<ShippingOption>(this._workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, this._storeContext.CurrentStore.Id);
            //generalResponseModel.Data = true;
            return true;
        }



        private bool OpcSavePaymentMethod2()
        {

            List<ShoppingCartItem> cart = (from sci in this._workContext.CurrentCustomer.ShoppingCartItems
                                           where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                                           select sci).LimitPerStore(this._storeContext.CurrentStore.Id).ToList<ShoppingCartItem>();
            int filterByCountryId = 0;
            bool flag = this._addressSettings.CountryEnabled && this._workContext.CurrentCustomer.BillingAddress != null && this._workContext.CurrentCustomer.BillingAddress.Country != null;
            if (flag)
            {
                filterByCountryId = this._workContext.CurrentCustomer.BillingAddress.Country.Id;
            }
            var value=_checkoutModelFactory.PreparePaymentMethodModel(cart, 0);
            //CheckoutPaymentMethodResponseModel value = this._checkoutModelFactoryApi.PreparePaymentMethodModel(cart, filterByCountryId);
            //return this.Ok(value);

            if (value.PaymentMethods.Count == 0)
            {
                return false;
            }
            string value2 = value.PaymentMethods.FirstOrDefault().PaymentMethodSystemName;

            //IActionResult result;
            try
            {
                List<ShoppingCartItem> list = (from sci in this._workContext.CurrentCustomer.ShoppingCartItems
                                               where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                                               select sci).LimitPerStore(this._storeContext.CurrentStore.Id).ToList<ShoppingCartItem>();
                //bool flag = list.Count == 0;
                //if (flag)
                //{
                //    throw new Exception("Your cart is empty");
                //}
                //string value2 = value.Value;
                //bool flag2 = string.IsNullOrEmpty(value2);
                //if (flag2)
                //{
                //    throw new Exception("Selected payment method can't be parsed");
                //}
                CheckoutPaymentMethodModel checkoutPaymentMethodModel = new CheckoutPaymentMethodModel();
                bool enabled = this._rewardPointsSettings.Enabled;
                if (enabled)
                {
                    this._genericAttributeService.SaveAttribute<bool>(this._workContext.CurrentCustomer, SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, checkoutPaymentMethodModel.UseRewardPoints, this._storeContext.CurrentStore.Id);
                }
                IPaymentMethod paymentMethod = this._paymentService.LoadPaymentMethodBySystemName(value2);
                bool flag3 = paymentMethod == null || !paymentMethod.IsPaymentMethodActive(this._paymentSettings) || !this._pluginFinder.AuthenticateStore(paymentMethod.PluginDescriptor, this._storeContext.CurrentStore.Id);
                if (flag3)
                {
                    //throw new Exception("Selected payment method can't be parsed");
                }
                this._genericAttributeService.SaveAttribute<string>(this._workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPaymentMethod, value2, this._storeContext.CurrentStore.Id);
                //GeneralResponseModel<bool> value3 = new GeneralResponseModel<bool>
                //{
                //    Data = true
                //};
                //result = this.Ok(value3);
            }
            catch (Exception ex)
            {
                this._logger.Warning(ex.Message, ex, this._workContext.CurrentCustomer);
                //result = this.Json(new
                //{
                //    error = 1,
                //    message = ex.Message
                //});
            }
            return true;
            //return result;
        }

        private bool OpcConfirmOrder(OrderDivisionModel model,out string msg)
        {
            try
            {
                msg = "";
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                CheckoutSaveAddressId2();
                CheckoutSetShippingMethods2();
                OpcSavePaymentMethod2();
                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                //if (processPaymentRequest == null)
                //{
                //    //Check whether payment workflow is required
                //    if (_orderProcessingService.IsPaymentWorkflowRequired(cart))
                //    {
                //        throw new Exception("Payment information is not entered");
                //    }

                    processPaymentRequest = new ProcessPaymentRequest();
                //}

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    
                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    _orderProcessingService.MarkOrderAsPaid(placeOrderResult.PlacedOrder);
                    string desc=string.Format("Amount Of Cash: {0}, Amount Of Dolor: {1}, Amount Of Gold: {2}", model.AmountOfCash.ToString(),model.AmountOfDaller.ToString(),model.AmountOfGold.ToString());
                        
                    placeOrderResult.PlacedOrder.OrderNotes.Add(new OrderNote
                    {
                        Note = desc,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    _orderService.UpdateOrder(placeOrderResult.PlacedOrder);
                    msg = "ثبت با موفقیت انجام شد";
                    return true;
                }
                foreach (var error in placeOrderResult.Errors)
                {
                    if (string.IsNullOrEmpty(msg))
                        msg = error;
                    else
                        msg += "\r\n" + error;
                }
                return false;
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                msg = "بروز خطا در زمان ثبت اطلاعات ";
                return false;
            }
        }

        public IActionResult OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("HomePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //get the order
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("HomePage");

                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("HomePage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("HomePage");

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    return RedirectToRoute("HomePage");

                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
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

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Content(exc.Message);
            }
        }

        #endregion

        private OrderDivisionModel getDivisionModel(List<ShoppingCartItem> cart)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newCheckOutSettings = _settingService.LoadSetting<NewCheckOutSettings>(storeScope);

            decimal orderTotalDiscountAmountBase;
            List<DiscountForCaching> _;
            List<AppliedGiftCard> appliedGiftCards;
            decimal redeemedRewardPointsAmount;
            int redeemedRewardPoints;
            var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, out orderTotalDiscountAmountBase, out _, out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
            string StrTotalPrice = "";
            if (shoppingCartTotalBase.HasValue)
            {
                var shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                StrTotalPrice = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
            }

            return new OrderDivisionModel()
            {
                AmountOfCash = 0,
                AmountOfDaller = 0,
                AmountOfGold = 0,
                BaseDallerPrice = newCheckOutSettings.DollerPrice,
                BaseGoldPriceInGram = newCheckOutSettings.GlodPriceInGram,
                TotalPrice = shoppingCartTotalBase.Value,
                Str_TotalPrice = StrTotalPrice
            };
        }
        private bool IsUserInRole()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var newCheckOutSettings = _settingService.LoadSetting<NewCheckOutSettings>(storeScope);
            if (newCheckOutSettings.RoleId == 0)
                return false;
            return _workContext.CurrentCustomer.GetCustomerRoleIds(true).Any(p => p == newCheckOutSettings.RoleId);
        }
       

    }
}