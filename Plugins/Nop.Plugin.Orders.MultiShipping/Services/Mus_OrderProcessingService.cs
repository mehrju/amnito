using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Factories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Nop.Plugin.Orders.MultiShipping.Services
{

    public class Mus_OrderProcessingService : OrderProcessingService, IExtnOrderProcessingService
    {

        #region Fields
        private readonly INotificationService _notificationService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly ISettingService _settingService;
        private readonly IHagheMaghar _hagheMaghar;
        private readonly IAddressService _addressService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductService _productService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ITaxService _taxService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IEncryptionService _encryptionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly IAffiliateService _affiliateService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPdfService _pdfService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ShippingSettings _shippingSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IRepository<ExnShippmentModel> _exnShippmentRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<FirstOrderModel> _firstOrdeRepository;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IRepository<Tbl_CancelReason_Order> _repositoryTbl_CancelReason_Order;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="orderService">Order service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="productService">Product service</param>
        /// <param name="paymentService">Payment service</param>
        /// <param name="logger">Logger</param>
        /// <param name="orderTotalCalculationService">Order total calculation service</param>
        /// <param name="priceCalculationService">Price calculation service</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <param name="productAttributeFormatter">Product attribute formatter</param>
        /// <param name="giftCardService">Gift card service</param>
        /// <param name="shoppingCartService">Shopping cart service</param>
        /// <param name="checkoutAttributeFormatter">Checkout attribute service</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shipmentService">Shipment service</param>
        /// <param name="taxService">Tax service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="discountService">Discount service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="customerActivityService">Customer activity service</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="affiliateService">Affiliate service</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="pdfService">PDF service</param>
        /// <param name="rewardPointService">Reward point service</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="countryService">Country service</param>
        /// <param name="paymentSettings">Payment settings</param>
        /// <param name="stateProvinceService">State province service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="orderSettings">Order settings</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="customNumberFormatter">Custom number formatter</param>
        public Mus_OrderProcessingService(
            IExtendedShipmentService extendedShipmentService,
            IRepository<OrderNote> orderNoteRepository,
            ISettingService settingService,
            IHagheMaghar hagheMaghar,
            IRepository<FirstOrderModel> firstOrdeRepository,
            IAddressService addressService,
            IAddressModelFactory addressModelFactory,
            IDbContext dbContext,
            IOrderService orderService,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductService productService,
            IPaymentService paymentService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter,
            IGiftCardService giftCardService,
            IShoppingCartService shoppingCartService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ITaxService taxService,
            ICustomerService customerService,
            IDiscountService discountService,
            IEncryptionService encryptionService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            ICurrencyService currencyService,
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher,
            IPdfService pdfService,
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ShippingSettings shippingSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings,
            ICustomNumberFormatter customNumberFormatter,
            IRepository<ExnShippmentModel> exnShippmentRepository
            , IRepository<Order> orderRepository
            , IStoreContext storeContext
            , INotificationService notificationService
            , IRepository<Tbl_CancelReason_Order> repositoryTbl_CancelReason_Order
            ) : base(orderService, webHelper,
             localizationService,
             languageService,
             productService,
             paymentService,
             logger,
             orderTotalCalculationService,
             priceCalculationService,
             priceFormatter,
             productAttributeParser,
             productAttributeFormatter,
             giftCardService,
             shoppingCartService,
             checkoutAttributeFormatter,
             shippingService,
             shipmentService,
             taxService,
             customerService,
             discountService,
             encryptionService,
             workContext,
             workflowMessageService,
             vendorService,
             customerActivityService,
             currencyService,
             affiliateService,
             eventPublisher,
             pdfService,
             rewardPointService,
             genericAttributeService,
             countryService,
             stateProvinceService,
             shippingSettings,
             paymentSettings,
             rewardPointsSettings,
             orderSettings,
             taxSettings,
             localizationSettings,
             currencySettings,
             customNumberFormatter)
        {
            _repositoryTbl_CancelReason_Order = repositoryTbl_CancelReason_Order;
            _notificationService = notificationService;
            _extendedShipmentService = extendedShipmentService;
            _orderNoteRepository = orderNoteRepository;
            _settingService = settingService;
            _hagheMaghar = hagheMaghar;
            _storeContext = storeContext;
            this._firstOrdeRepository = firstOrdeRepository;
            this._orderRepository = orderRepository;
            this._addressModelFactory = addressModelFactory;
            this._addressService = addressService;
            this._dbContext = dbContext;
            this._exnShippmentRepository = exnShippmentRepository;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._productService = productService;
            this._paymentService = paymentService;
            this._logger = logger;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._giftCardService = giftCardService;
            this._shoppingCartService = shoppingCartService;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._taxService = taxService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._encryptionService = encryptionService;
            this._customerActivityService = customerActivityService;
            this._currencyService = currencyService;
            this._affiliateService = affiliateService;
            this._eventPublisher = eventPublisher;
            this._pdfService = pdfService;
            this._rewardPointService = rewardPointService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;

            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._localizationSettings = localizationSettings;
            this._currencySettings = currencySettings;
            this._customNumberFormatter = customNumberFormatter;
        }

        #endregion

        public bool InsertCanceledShipment(Shipment shipment)
        {
            try
            {
                string Query = @"
                        IF NOT EXISTS(SELECT
	                        TOP(1) 1
                        FROM
	                        dbo.Tb_DeletedTrackingNumber AS TDTN
                        WHERE
	                        TDTN.TrackingNumber = '" + shipment.TrackingNumber + @"')
                        BEGIN
                            INSERT INTO dbo.Tb_DeletedTrackingNumber
                            (
	                            TrackingNumber
	                            , orderId
	                            , shipmentId
	                            , deleteDate
	                            , CountryId
	                            , StateId
                            )
                            VALUES
                            (	N'" + shipment.TrackingNumber + @"' -- TrackingNumber - nvarchar(30)
	                            , " + shipment.OrderId.ToString() + @"
	                            ," + shipment.Id + @"
	                            , GETDATE() 
	                            , NULL 
	                            , NULL 
	                            )
                        END 
                        ELSE
                        BEGIN
	                      UPDATE dbo.Tb_DeletedTrackingNumber SET TrackingNumber = '" + shipment.TrackingNumber + "' WHERE TrackingNumber = '" + shipment.TrackingNumber + @"'
                        END ";
                int result = _dbContext.ExecuteSqlCommand(Query, parameters: new object[0]);
                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                Log("خطا در زمان ثبت کنسلی بارکد  " + shipment.TrackingNumber, ex.Message + (ex.InnerException != null ? "==>" + ex.InnerException.Message : ""));
            }

            return false;
        }
        public void InsertOrderNote(string note, int orderId)
        {
            OrderNote Onote = new OrderNote()
            {
                Note = note + "-" + _workContext.CurrentCustomer.Id.ToString(),
                CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                DisplayToCustomer = false,
                OrderId = orderId
            };
            _orderNoteRepository.Insert(Onote);
        }
        public bool HasDateCollect(Shipment shipment)
        {
            string query = @"SELECT
	                    SA.DataCollect
                    FROM
	                    dbo.ShipmentAppointment AS SA
                    WHERE
	                    SA.ShipmentId =" + shipment.Id;
            var data = _dbContext.SqlQuery<DateTime?>(query, new object[0]).FirstOrDefault();
            if (data.HasValue)
                return true;
            return false;
        }
        public PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments = null)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer ShippingAddress
                var orderCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (!CommonHelper.IsValidEmail(orderCustomer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.BillingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.BillingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);

                }
                int sendertStateId = 0;
                if (_extendedShipmentService.getDefulteSenderState(orderCustomer.Id, out sendertStateId))
                {
                    orderCustomer.BillingAddress.StateProvinceId = sendertStateId;
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);
                }

                var address = _addressService.GetAddressById(shipments[0].ShippmentAddressId);
                orderCustomer.ShippingAddress = address;
                _customerService.UpdateCustomer(orderCustomer);
                if (!CommonHelper.IsValidEmail(orderCustomer.ShippingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        address.Email = orderCustomer.Email;
                    }
                    else
                    {
                        address.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.ShippingAddress);

                }


                #endregion

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);
                #region HagheMaghar
                var HagheMagharPrice = CalcHagheMaghar(details, processPaymentRequest);
                #endregion
                CalcAgentSaleAmount(details, customer);
                #region CheckWallet
                if (processPaymentRequest.PaymentMethodSystemName == "Payments.CashOnDelivery")
                {
                    int rewardPointsBalance =
                        _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                    if (rewardPointsBalance < 1500000)
                    {
                        return new PlaceOrderResult()
                        {
                            Errors = new List<string>() { "موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد" },
                            PlacedOrder = null
                        };
                    }
                }
                #endregion
                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);
                    if (HagheMagharPrice.Item1 > 0)
                    {
                        _hagheMaghar.Insert(order.OrderItems.First().Id, HagheMagharPrice.Item1, HagheMagharPrice.Item2);
                    }
                    //check first order and add first time order tag
                    CheckFirstOrder(order);

                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }
                    _extendedShipmentService.MarkOrder(OrderRegistrationMethod.Normal, order);

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                    //Add Shipment(s)
                    SaveOrderShipments(details, order, shipments);

                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
                return result;

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);

            return result;
        }
        public PlaceOrderResult PlaceOrderApi(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments, int? registrationMethod, int bulkorderId = 0)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer Shipping Address
                var orderCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (!CommonHelper.IsValidEmail(orderCustomer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.BillingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.BillingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);

                }
                var ShippmentAddress = _addressService.GetAddressById(shipments[0].ShippmentAddressId);

                orderCustomer.ShippingAddress = ShippmentAddress;
                _customerService.UpdateCustomer(orderCustomer);
                if (!CommonHelper.IsValidEmail(orderCustomer.ShippingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.ShippingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.ShippingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.ShippingAddress);
                }
                int sendertStateId = 0;
                if (_extendedShipmentService.getDefulteSenderState(orderCustomer.Id, out sendertStateId))
                {
                    orderCustomer.BillingAddress.StateProvinceId = sendertStateId;
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);
                }
                #endregion

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

                #region HagheMaghar

                var HagheMagharPrice = CalcHagheMaghar(details, processPaymentRequest);
                #endregion

                CalcAgentSaleAmount(details, customer);
                #region Check Wallet
                if (processPaymentRequest.PaymentMethodSystemName != "Payments.CashOnDelivery")
                {
                    if (orderCustomer.BillingAddress.FirstName.ToLower() != "test-api"
                            && orderCustomer.BillingAddress.LastName != "be-canceled")
                    {
                        //int rewardPointsBalance =
                        //    _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                        //if (details.OrderTotal > rewardPointsBalance)
                        //{
                        //    return new PlaceOrderResult()
                        //    {
                        //        Errors = new List<string>() { "موجودی کیف پول برای این سفارش می بایست حداقل "+  Convert.ToInt32(details.OrderTotal).ToString("N0")
                        //                                                                                 + " ريال باشد. موجودی فعلی "+ rewardPointsBalance.ToString("N0") + " erroCode:930" },
                        //        PlacedOrder = null
                        //    };
                        //}
                        //else
                        //{
                        //    details.RedeemedRewardPointsAmount = Convert.ToInt32(details.OrderTotal);
                        //    details.RedeemedRewardPoints = Convert.ToInt32(details.OrderTotal);
                        //    details.OrderTotal = 0;
                        //    processPaymentRequest.PaymentMethodSystemName = null;
                        //}
                    }
                    else
                    {
                        details.OrderTotal = 0;
                        processPaymentRequest.PaymentMethodSystemName = null;
                    }
                }
                else
                {
                    int rewardPointsBalance =
                        _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                    if (rewardPointsBalance < 1500000)
                    {
                        return new PlaceOrderResult()
                        {
                            Errors = new List<string>() { "موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد erroCode:940" },
                            PlacedOrder = null
                        };
                    }
                }
                #endregion

                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);
                    if (HagheMagharPrice.Item1 > 0)
                    {
                        _hagheMaghar.Insert(order.OrderItems.First().Id, HagheMagharPrice.Item1, HagheMagharPrice.Item2);
                    }
                    //check first order and add first time order tag
                    CheckFirstOrder(order);
                    if (bulkorderId > 0)
                    {
                        UpdateBulkOrder(bulkorderId, order.Id);
                    }
                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }
                    //Add Shipment(s)
                    SaveOrderShipments(details, order, shipments);
                    if (registrationMethod != null)
                    {
                        _extendedShipmentService.MarkOrder((OrderRegistrationMethod)registrationMethod.Value, order);
                    }
                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);


                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
                return result;

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);

            return result;
        }
        private bool UpdateBulkOrder(int bulkOrder, int OrderId)
        {
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "BulkOrderId", SqlDbType = SqlDbType.Int,Value = bulkOrder },
                new SqlParameter() { ParameterName = "OrderId", SqlDbType = SqlDbType.Int,Value = OrderId }
            };
            return _dbContext.SqlQuery<bool?>(@"EXEC dbo.Sp_UpdateBulkOrder @BulkOrderId, @OrderId", prms).FirstOrDefault().GetValueOrDefault(false);
        }

        public PlaceOrderResult PlaceOrderApi_Postkhone(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments, int? registrationMethod, int bulkorderId = 0)
        {
            long total = 0;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer Shipping Address
                var orderCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (!CommonHelper.IsValidEmail(orderCustomer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.BillingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.BillingAddress.Email = processPaymentRequest.StoreId == 3 ? "fake@postbar.ir" : "fake@postex.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);

                }
                var ShippmentAddress = _addressService.GetAddressById(shipments[0].ShippmentAddressId);

                orderCustomer.ShippingAddress = ShippmentAddress;
                _customerService.UpdateCustomer(orderCustomer);
                if (!CommonHelper.IsValidEmail(orderCustomer.ShippingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.ShippingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.ShippingAddress.Email = processPaymentRequest.StoreId == 3 ? "fake@postbar.ir" : "fake@postex.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.ShippingAddress);
                }
                int sendertStateId = 0;
                if (_extendedShipmentService.getDefulteSenderState(orderCustomer.Id, out sendertStateId))
                {
                    orderCustomer.BillingAddress.StateProvinceId = sendertStateId;
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);
                }
                #endregion
                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);
                #region HagheMaghar

                var HagheMagharPrice = CalcHagheMaghar(details, processPaymentRequest);
                #endregion
                CalcAgentSaleAmount(details, customer);
                #region Check Wallet

                if ((string.IsNullOrEmpty(processPaymentRequest.PaymentMethodSystemName))
                    && registrationMethod != 2)
                {
                    if (orderCustomer.BillingAddress.FirstName.ToLower() != "test-api"
                            && orderCustomer.BillingAddress.LastName != "be-canceled")
                    {
                        int rewardPointsBalance =
                            _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                        if (details.OrderTotal > rewardPointsBalance)
                        {
                            return new PlaceOrderResult()
                            {
                                Errors = new List<string>() { "موجودی کیف پول برای این سفارش می بایست حداقل "+  Convert.ToInt32(details.OrderTotal).ToString("N0")
                                                                                                         + " ريال باشد. موجودی فعلی "+ rewardPointsBalance.ToString("N0") + " erroCode:930" },
                                PlacedOrder = null
                            };
                        }
                        else
                        {
                            details.RedeemedRewardPointsAmount = Convert.ToInt32(details.OrderTotal);
                            details.RedeemedRewardPoints = Convert.ToInt32(details.OrderTotal);
                            details.OrderTotal = 0;
                            processPaymentRequest.PaymentMethodSystemName = null;
                        }
                    }
                    else
                    {
                        details.OrderTotal = 0;
                        processPaymentRequest.PaymentMethodSystemName = null;
                    }
                }
                else if (processPaymentRequest.PaymentMethodSystemName == "Payments.CashOnDelivery")
                {
                    int rewardPointsBalance =
                        _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                    if (rewardPointsBalance < 1500000)
                    {
                        return new PlaceOrderResult()
                        {
                            Errors = new List<string>() { "موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد erroCode:940" },
                            PlacedOrder = null
                        };
                    }
                }
                #endregion
                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");
                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;
                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);
                    if (HagheMagharPrice.Item1 > 0)
                    {
                        _hagheMaghar.Insert(order.OrderItems.First().Id, HagheMagharPrice.Item1, HagheMagharPrice.Item2);
                    }
                    //check first order and add first time order tag
                    CheckFirstOrder(order);
                    if (bulkorderId > 0)
                    {
                        UpdateBulkOrder(bulkorderId, order.Id);
                    }
                    //apply First Order Discount
                    if (!(order.Customer.Id == 8186075 || order.Customer.AffiliateId == 1186) && result.PlacedOrder.OrderDiscount == 0)
                    {
                        if (_extendedShipmentService.IsPostService(order.OrderItems.First().Product.ProductCategories.First().CategoryId))
                        {
                            bool CanUseFirstDiscount = (CheckFirstOrder(order) || _extendedShipmentService.IsInValidDiscountPeriod(order)) && _extendedShipmentService.CanUseFirstOrderDiscount(order);
                            if (CanUseFirstDiscount)
                            {
                                var firstOrderDiscount = _extendedShipmentService.getFistOrderDiscount();
                                if (firstOrderDiscount > 0)
                                {
                                    result.PlacedOrder.OrderDiscount = (int)((((float)result.PlacedOrder.OrderTotal * firstOrderDiscount) / 100));
                                    result.PlacedOrder.OrderTotal = result.PlacedOrder.OrderTotal - result.PlacedOrder.OrderDiscount;
                                    _orderRepository.Update(result.PlacedOrder);
                                    InsertOrderNote($"شامل {firstOrderDiscount} درصد تخفیف سفارش اول می باشد", result.PlacedOrder.Id);
                                }
                            }
                        }
                    }
                    else if (new DateTime(2021, 01, 19).CompareTo(DateTime.Now) > 0)// اتاق ایران اتریش
                    {
                        result.PlacedOrder.OrderDiscount = (int)((((float)result.PlacedOrder.OrderTotal * 10) / 100));
                        result.PlacedOrder.OrderTotal = result.PlacedOrder.OrderTotal - result.PlacedOrder.OrderDiscount;
                        _orderRepository.Update(result.PlacedOrder);
                        InsertOrderNote($"شامل {10} درصد تخفیف اتاق ایران-اتریش می باشد", result.PlacedOrder.Id);
                    }
                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    //SaveGiftCardUsageHistory(details, order);

                    ////recurring orders
                    //if (details.IsRecurringShoppingCart)
                    //{
                    //    CreateFirstRecurringPayment(processPaymentRequest, order);
                    //}
                    //Add Shipment(s)
                    SaveOrderShipments(details, order, shipments);
                    bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;

                    if (registrationMethod != null || isFromBidoc)
                    {
                        _extendedShipmentService.MarkOrder(isFromBidoc ? OrderRegistrationMethod.bidok : (OrderRegistrationMethod)registrationMethod.Value, order);
                    }
                    //notifications
                    SendNotificationsAndSaveNotes(order);
                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                    watch.Stop();
                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));

                if (result.Success)
                {
                    var order = result.PlacedOrder;
                    List<string> strError = new List<string>();

                    int ServiceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;
                    bool IsFromAp = false;
                    bool newPricePolticAccess = isFromBidoc || IsFromAp;
                    string Error = "";
                    if (!newPricePolticAccess)
                    {
                        if (new int[] { 654, 655, 660, 661, 662, 698, 697, 696, 695, 694, 693, 691, 690,725,726,727 }.Contains(ServiceId))// && IsTwoStepOrder(order))//post
                        {
                            //  _extendedShipmentService._GenerateBarcodes(order, out strError);
                            //   InsertOrderNote(Error + "," + (string.Join(",", strError) ?? ""), order.Id);
                        }
                        else
                        {
                            if (!_extendedShipmentService.CheckHasValidPrice(order))
                            {
                                if (!IsFromAp)
                                    result.Errors.Add($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {02191300250} تماس بگیرید. شماره سفارش:{result.PlacedOrder.Id}");
                                else
                                    result.Errors.Add($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {021 - 83333} تماس بگیرید و سپس در قسمت \"مرسولات من\" اقدام به پرداخت کنید. شماره سفارش:{result.PlacedOrder.Id}");
                                return new PlaceOrderResult()
                                {
                                    Errors = result.Errors,
                                    PlacedOrder = result.PlacedOrder
                                };
                            }
                            else if (ServiceId == 717)
                            {
                                string snapOrder = _extendedShipmentService.RegisterSnappbox_Order(order).Result;
                                if (!string.IsNullOrEmpty(snapOrder))
                                {
                                    result.Errors.Add(snapOrder);
                                    return new PlaceOrderResult()
                                    {
                                        Errors = result.Errors,
                                        PlacedOrder = result.PlacedOrder
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!_extendedShipmentService.CheckHasValidPrice(order))
                        {
                            if (!IsFromAp)
                                result.Errors.Add($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {02191300250} تماس بگیرید. شماره سفارش:{result.PlacedOrder.Id}");
                            else
                                result.Errors.Add($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {021 - 83333} تماس بگیرید و سپس در قسمت \"مرسولات من\" اقدام به پرداخت کنید. شماره سفارش:{result.PlacedOrder.Id}");
                            return new PlaceOrderResult()
                            {
                                Errors = result.Errors,
                                PlacedOrder = result.PlacedOrder
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error("مربوط به ثبت سفارش ===>" + exc.Message, exc);
                result.AddError(exc.Message);
            }
            if (result.Success)
                return result;
            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);
            return result;
        }
        public PlaceOrderResult PlaceOrderNewCheckOut(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments, float? Senderlat, float? SenderLon,
            bool IsFromAp = false, bool IsFromSep = false)
        {

            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer Shipping Address
                var orderCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (!CommonHelper.IsValidEmail(orderCustomer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.BillingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.BillingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);

                }
                int sendertStateId = 0;
                if (_extendedShipmentService.getDefulteSenderState(orderCustomer.Id, out sendertStateId))
                {
                    orderCustomer.BillingAddress.StateProvinceId = sendertStateId;
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);
                }
                var ShippmentAddress = _addressService.GetAddressById(shipments[0].ShippmentAddressId);

                if (!CommonHelper.IsValidEmail(orderCustomer.ShippingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.ShippingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.ShippingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.ShippingAddress);
                }
                #endregion

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);
                #region HagheMaghar

                var HagheMagharPrice = CalcHagheMaghar(details, processPaymentRequest);
                #endregion

                CalcAgentSaleAmount(details, customer);
                #region Check Wallet
                if (processPaymentRequest.PaymentMethodSystemName == "Payments.CashOnDelivery")
                {
                    int rewardPointsBalance =
                        _rewardPointService.GetRewardPointsBalance(details.Customer.Id, _storeContext.CurrentStore.Id);
                    if (rewardPointsBalance < 1500000)
                    {
                        return new PlaceOrderResult()
                        {
                            Errors = new List<string>() { "موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد erroCode:940" },
                            PlacedOrder = null
                        };
                    }
                }
                #endregion

                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    if (Senderlat.HasValue && SenderLon.HasValue)
                        _extendedShipmentService.InsertAddressLocation(order.BillingAddressId, Senderlat.Value, SenderLon.Value);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);
                    if (HagheMagharPrice.Item1 > 0)
                    {
                        _hagheMaghar.Insert(order.OrderItems.First().Id, HagheMagharPrice.Item1, HagheMagharPrice.Item2);
                    }
                    //apply First Order Discount
                    if (_extendedShipmentService.IsPostService(order.OrderItems.First().Product.ProductCategories.First().CategoryId) && result.PlacedOrder.OrderDiscount == 0)
                    {
                        bool CanUseFirstDiscount = (CheckFirstOrder(order) || _extendedShipmentService.IsInValidDiscountPeriod(order)) && _extendedShipmentService.CanUseFirstOrderDiscount(order);
                        if (CanUseFirstDiscount)
                        {
                            var firstOrderDiscount = _extendedShipmentService.getFistOrderDiscount();
                            if (firstOrderDiscount > 0)
                            {
                                result.PlacedOrder.OrderDiscount = (int)((((float)result.PlacedOrder.OrderTotal * firstOrderDiscount) / 100));
                                result.PlacedOrder.OrderTotal = result.PlacedOrder.OrderTotal - result.PlacedOrder.OrderDiscount;
                                _orderRepository.Update(result.PlacedOrder);
                                InsertOrderNote($"شامل {firstOrderDiscount} درصد تخفیف سفارش اول می باشد", result.PlacedOrder.Id);
                            }
                        }
                    }
                    if (IsFromSep && result.PlacedOrder.OrderDiscount == 0)
                    {
                        int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                        int discountPercent = 0;
                        if (_extendedShipmentService.IsPostService(serviceId))// پست
                        {
                            discountPercent = 20;
                        }
                        else if (serviceId == 712 || serviceId == 714 || serviceId == 702)// چاپار و یارباکس
                        {
                            discountPercent = 5;
                        }
                        else if (serviceId == 707 || serviceId == 710 || serviceId == 719)//پست خارجی
                        {
                            discountPercent = 2;
                        }
                        result.PlacedOrder.OrderDiscount = (int)((((float)result.PlacedOrder.OrderTotal * discountPercent) / 100));
                        result.PlacedOrder.OrderTotal = result.PlacedOrder.OrderTotal - result.PlacedOrder.OrderDiscount;
                        _orderRepository.Update(result.PlacedOrder);
                        InsertOrderNote(string.Format("شامل {0} درصد تخفیف اتاق ایران-اتریش می باشد", discountPercent), result.PlacedOrder.Id);
                    }
                    //else if (new DateTime(2021, 01, 19).CompareTo(DateTime.Now) > 0 && result.PlacedOrder.OrderDiscount == 0)// اتاق ایران اتریش
                    //{
                    //    result.PlacedOrder.OrderDiscount = (int)((((float)result.PlacedOrder.OrderTotal * 10) / 100));
                    //    result.PlacedOrder.OrderTotal = result.PlacedOrder.OrderTotal - result.PlacedOrder.OrderDiscount;
                    //    _orderRepository.Update(result.PlacedOrder);
                    //    InsertOrderNote($"شامل {10} درصد تخفیف اتاق ایران-اتریش می باشد", result.PlacedOrder.Id);
                    //}
                    
                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }
                    //Add Shipment(s)
                    SaveOrderShipments(details, order, shipments);
                    bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;

                    _extendedShipmentService.MarkOrder((IsFromAp ? OrderRegistrationMethod.Ap :
                        (isFromBidoc ? OrderRegistrationMethod.bidok : (IsFromSep ? OrderRegistrationMethod.Sep : OrderRegistrationMethod.NewUi))), order);
                    if (IsFromAp)
                    {
                        _notificationService.sendSms("سفارش جدید از سیستم آپ .شماره سفارش:  " + order.Id, "09129427467");
                    }
                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));


                if (result.Success)
                {
                    var order = result.PlacedOrder;
                    List<string> strError = new List<string>();

                    int ServiceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    string Error = "";
                    bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;
                    bool AccessToNewPricePolitic = IsFromAp || isFromBidoc;
                    if (!AccessToNewPricePolitic)
                    {
                        if (new int[] { 654, 655, 660, 661, 662, 698, 697, 696, 695, 694, 693, 691, 690,725,726,727 }.Contains(ServiceId))// && IsTwoStepOrder(order))//post
                        {
                            //  _extendedShipmentService._GenerateBarcodes(order, out strError);
                            //   InsertOrderNote(Error + "," + (string.Join(",", strError) ?? ""), order.Id);
                        }
                        else
                        {
                            if (!_extendedShipmentService.CheckHasValidPrice(order))
                            {
                                if (!IsFromAp)
                                    result.Errors.Add($"سفارش شما با موفقیت ثبت شد.\r\n شماره سفارش {result.PlacedOrder.Id} \r\n در ادامه کارشناسان ما با شما تماس خواهند گرفت و پس از آن در قسمت حساب من اقدام به پراخت نمایید");
                                else
                                    result.Errors.Add($"سفارش شما با موفقیت ثبت شد.\r\n شماره سفارش {result.PlacedOrder.Id} \r\n در ادامه کارشناسان ما با شما تماس خواهند گرفت و پس از آن در قسمت \"مرسولات من\" اقدام به پراخت نمایید" + "\r\n" + "021-83333");
                                return new PlaceOrderResult()
                                {
                                    Errors = result.Errors,
                                    PlacedOrder = result.PlacedOrder
                                };
                            }
                            else if (ServiceId == 717)
                            {
                                string snapOrder = _extendedShipmentService.RegisterSnappbox_Order(order).Result;
                                if (!string.IsNullOrEmpty(snapOrder))
                                {
                                    result.Errors.Add(snapOrder);
                                    return new PlaceOrderResult()
                                    {
                                        Errors = result.Errors,
                                        PlacedOrder = result.PlacedOrder
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!_extendedShipmentService.CheckHasValidPrice(order))
                        {
                            if (!IsFromAp)
                                result.Errors.Add($"سفارش شما با موفقیت ثبت شد.\r\n شماره سفارش {result.PlacedOrder.Id} \r\n در ادامه کارشناسان ما با شما تماس خواهند گرفت و پس از آن در قسمت \"سفارشات من\" اقدام به پراخت نمایید");
                            else
                                result.Errors.Add($"سفارش شما با موفقیت ثبت شد.\r\n شماره سفارش {result.PlacedOrder.Id} \r\n در ادامه کارشناسان ما با شما تماس خواهند گرفت و پس از آن در قسمت \"مرسولات من\" اقدام به پراخت نمایید" + "\r\n" + "021-83333");
                            return new PlaceOrderResult()
                            {
                                Errors = result.Errors,
                                PlacedOrder = result.PlacedOrder
                            };
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }
            if (result.Success)
                return result;
            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);
            return result;
        }
        public List<string> EndOfOrderPlaced(Order order, int? registrationMethod=1, int bulkorderId = 0, bool IsFromAp = false, bool IsFromSep = false)
        {

            {
                List<string> strError = new List<string>();

                int ServiceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                bool isFromBidoc = _workContext.CurrentCustomer.Id == 4144899 || _workContext.CurrentCustomer.AffiliateId == 1149;
                bool newPricePolticAccess = isFromBidoc || IsFromAp;
                string Error = "";
                if (!newPricePolticAccess)
                {
                    if (new int[] { 654, 655, 660, 661, 662, 698, 697, 696, 695, 694, 693, 691, 690 ,725,726,727}.Contains(ServiceId))// && IsTwoStepOrder(order))//post
                    {
                        //  _extendedShipmentService._GenerateBarcodes(order, out strError);
                        //   InsertOrderNote(Error + "," + (string.Join(",", strError) ?? ""), order.Id);
                    }
                    else
                    {
                        if (!_extendedShipmentService.CheckHasValidPrice(order))
                        {
                            if (!IsFromAp)
                                return new List<string>() { ($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {02191300250} تماس بگیرید. شماره سفارش:{order.Id}")+"|17" };
                            else
                                return new List<string>() { ($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {021 - 83333} تماس بگیرید و سپس در قسمت \"مرسولات من\" اقدام به پرداخت کنید. شماره سفارش:{order.Id}")+"|17" };
                        }
                        else if (ServiceId == 717)
                        {
                            string snapOrder = _extendedShipmentService.RegisterSnappbox_Order(order).Result;
                            if (!string.IsNullOrEmpty(snapOrder))
                            {
                                return new List<string>() { snapOrder };
                            }
                        }
                    }
                }
                else
                {
                    if (!_extendedShipmentService.CheckHasValidPrice(order))
                    {
                        if (!IsFromAp)
                            return new List<string>() { ($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {02191300250} تماس بگیرید. شماره سفارش:{order.Id}")+"|17" };
                        else
                            return new List<string>() { ($"فاکتور شما نیاز به  تایید دارد. قبل از پرداخت با کارشناسان واحد پشتیبانی {021 - 83333} تماس بگیرید و سپس در قسمت \"مرسولات من\" اقدام به پرداخت کنید. شماره سفارش:{order.Id}")+"|17" };
                    }
                }
            }

            if (registrationMethod == 3 && order.PaymentMethodSystemName != "Payments.CashOnDelivery")
            {
                int rewardPointsBalance =
                                _rewardPointService.GetRewardPointsBalance(order.Customer.Id, _storeContext.CurrentStore.Id);
                if (order.OrderTotal > rewardPointsBalance)
                {
                   return new List<string>() { "موجودی کیف پول برای این سفارش می بایست حداقل "+  Convert.ToInt32(order.OrderTotal).ToString("N0")
                                                                                                         + " ريال باشد. موجودی فعلی "+ rewardPointsBalance.ToString("N0") + "|10" };
                }
                else
                {
                    int orderTotal = Convert.ToInt32(order.OrderTotal);
                    order.OrderTotal = 0;
                    order.PaymentMethodSystemName = null;
                    order.PaymentStatus = PaymentStatus.Paid;
                    _orderService.UpdateOrder(order);
                    _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -orderTotal, order.StoreId, "آزاد شدن برای سفارش شماره" + order.Id
                        , order, order.OrderTotal);
                }
            }
            CheckFirstOrder(order);
            if (bulkorderId > 0)
            {
                UpdateBulkOrder(bulkorderId, order.Id);
            }
            SendNotificationsAndSaveNotes(order);

            _customerService.ResetCheckoutData(order.Customer, order.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
            _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

            //check order status
            CheckOrderStatus(order);

            //raise event       
            _eventPublisher.Publish(new OrderPlacedEvent(order));
            Log("OrderPlacedEvent", "");
            if (order.PaymentStatus == PaymentStatus.Paid)
                ProcessOrderPaid(order);

            
            return new List<string>();
        }

        public PlaceOrderResult PlaceOrderWallet(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer Shipping Address
                var orderCustomer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
                if (!CommonHelper.IsValidEmail(orderCustomer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(orderCustomer.Email))
                    {
                        orderCustomer.BillingAddress.Email = orderCustomer.Email;
                    }
                    else
                    {
                        orderCustomer.BillingAddress.Email = "fake@postbar.ir";
                    }
                    _addressService.UpdateAddress(orderCustomer.BillingAddress);

                }
                #endregion

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);

                    //check first order and add first time order tag
                    CheckFirstOrder(order);

                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }
                    //Add Shipment(s)

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
            {
                List<string> strError = new List<string>();
                InsertOrderNote(string.Join(",", strError), result.PlacedOrder.Id);
            }

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);
            return result;
        }
        public PlaceOrderResult PlaceOrderCarton(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                #region Customer Shipping Address
                if (!CommonHelper.IsValidEmail(customer.BillingAddress.Email))
                {
                    if (CommonHelper.IsValidEmail(customer.Email))
                    {
                        customer.BillingAddress.Email = customer.Email;
                    }
                    else
                    {
                        customer.BillingAddress.Email = customer.Username;
                    }
                    _addressService.UpdateAddress(customer.BillingAddress);

                }
                if(customer.BillingAddress != null)
                {
                    customer.BillingAddress.CreatedOnUtc = DateTime.UtcNow;
                    _addressService.UpdateAddress(customer.BillingAddress);
                }
                #endregion

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);
                if(details.BillingAddress != null)
                    details.BillingAddress.CreatedOnUtc = DateTime.UtcNow;
                if(details.ShippingAddress != null)
                    details.ShippingAddress.CreatedOnUtc = DateTime.UtcNow;
                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);

                    //check first order and add first time order tag


                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }
                    //Add Shipment(s)

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));
                    Log("OrderPlacedEvent", "");
                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
            {
                List<string> strError = new List<string>();
                InsertOrderNote(string.Join(",", strError), result.PlacedOrder.Id);
            }

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            _logger.Error(logError, customer: customer);
            return result;
        }
        private bool CalcAgentSaleAmount(PlaceOrderContainer details, Customer customer)
        {
            int AgentSaleAmount = 0;
            foreach (var item in details.Cart)
            {
                var Lst_PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                if (Lst_PAM.Any(p => p.ProductAttribute.Name.Contains("ارزش افزوده")))
                {
                    var pam = Lst_PAM.FirstOrDefault(p => p.ProductAttribute.Name.Contains("ارزش افزوده"));
                    if (details.AppliedDiscounts.Any())
                    {
                        item.AttributesXml = _productAttributeParser.RemoveProductAttribute(item.AttributesXml, pam);
                        return false;
                    }
                    var txtPrice = _productAttributeParser.ParseValues(item.AttributesXml, pam.Id).FirstOrDefault();
                    if (txtPrice.ToEnDigit() > 0
                        && ((!customer.IsInCustomerRole("mini-Administrators")
                        && !customer.IsInCustomerRole("Administrators"))))
                    {
                        txtPrice = "0";
                        item.AttributesXml = _productAttributeParser.RemoveProductAttribute(item.AttributesXml, pam);
                    }
                    AgentSaleAmount += (txtPrice.ToEnDigit()) * item.Quantity;
                }
            }
            if (AgentSaleAmount > 0)
            {
                if (details.OrderTotal > 0)
                {
                    details.OrderTotal += AgentSaleAmount + ((AgentSaleAmount * 9) / 100);
                    details.OrderTaxTotal += ((AgentSaleAmount * 9) / 100);
                }
                else if (details.RedeemedRewardPointsAmount > 0)
                {
                    details.RedeemedRewardPoints += (AgentSaleAmount + ((AgentSaleAmount * 9) / 100));
                }
                return true;
            }
            return false;
        }
        private bool IsTwoStepOrder(Order order)
        {
            string Query = $@"SELECT DISTINCT
	                            TCI.*
                            FROM
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.OrderItem AS OI ON OI.OrderId = O.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
	                            INNER JOIN dbo.Category AS C ON C.Id = PCM.CategoryId
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE
	                            O.Id = {order.Id}";
            var data = _dbContext.SqlQuery<CategoryInfoModel>(Query, new object[0]).ToList();
            if (data.Any(p => p.IsTwoStep == false))
                return false;
            return true;

        }
        private (int, int) CalcHagheMaghar(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest)
        {
            int HagheMagharPrice = 0;
            int ShipmentHagheMaghar = 0;
            //long Total = 0;
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            #region If Is Wallet
            //var setting =
            //        _settingService.GetSetting("NopMaster.Wallet_ProductId", _storeContext.CurrentStore.Id, false);
            //int productId = setting == null ? 0 : int.Parse(setting.Value);
            //if (productId != 0)
            if (details.Cart.Any(p => p.ProductId == 10277))
                return (0, 0);
            #endregion
            int serviceId = details.Cart.SelectMany(p => p.Product.ProductCategories.Select(d => d.CategoryId)).First();
            var categoryInfo = _extendedShipmentService.GetCategoryInfo(serviceId);
            //_extendedShipmentService.RestartStopwatch(watch, $"از اول حق مقر تا گرفتن اطلاعات سرویس", ref Total);

            if (!categoryInfo.HasHagheMaghar)
                return (0, 0);
            int totalWeight = 0;
            int InTrafficAreaPrice = 0;
            List<int> ShipmentWeight = new List<int>();
            foreach (var item in details.Cart)
            {
                var PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                var weight = PAM.Where(p => p.ProductAttribute.Name == "وزن دقیق").FirstOrDefault();
                int WeightValue_t = 0;
                int WeightValue = 0;
                if (weight != null)
                {
                    var _WeightValue = _productAttributeParser.ParseValues(item.AttributesXml, weight.Id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(_WeightValue))
                    {
                        WeightValue = _WeightValue.ToEnDigit();
                        WeightValue_t = WeightValue * item.Quantity;
                    }
                }

                var _Width = PAM.Where(p => p.ProductAttribute.Name == "عرض مرسوله").FirstOrDefault();
                var _length = PAM.Where(p => p.ProductAttribute.Name == "طول مرسوله").FirstOrDefault();
                var _height = PAM.Where(p => p.ProductAttribute.Name == "ارتفاع مرسوله").FirstOrDefault();
                var InTRafficArea = PAM.Where(p => p.ProductAttribute.Name == "نیاز به طرح ترافیک؟").FirstOrDefault();

                if (InTRafficArea != null && InTrafficAreaPrice == 0)
                {
                    var _InTRafficArea = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml, InTRafficArea.Id).FirstOrDefault();
                    if (_InTRafficArea != null)
                    {
                        InTrafficAreaPrice = Convert.ToInt32(_InTRafficArea.PriceAdjustment);
                    }
                }
                int WeightValue_v = 0;
                if (_Width != null && _height != null && _height != null)
                {
                    var _WidthValue = _productAttributeParser.ParseValues(item.AttributesXml, _Width.Id).FirstOrDefault();
                    var _lengthValue = _productAttributeParser.ParseValues(item.AttributesXml, _length.Id).FirstOrDefault();
                    var _heightValue = _productAttributeParser.ParseValues(item.AttributesXml, _height.Id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(_WidthValue) && !string.IsNullOrEmpty(_lengthValue) && !string.IsNullOrEmpty(_heightValue))
                    {
                        WeightValue_v = ((_WidthValue.ToEnDigit() * _heightValue.ToEnDigit() * _lengthValue.ToEnDigit()) / 6000);
                        WeightValue_v = WeightValue_v * 1000;
                        if (WeightValue_v * item.Quantity > WeightValue_t)
                            WeightValue_t = (WeightValue_v * item.Quantity);
                    }

                }
                for (int i = 0; i < item.Quantity; i++)
                {
                    if (WeightValue > WeightValue_v)
                        ShipmentWeight.Add(WeightValue);
                    else
                        ShipmentWeight.Add(WeightValue_v);
                }
                totalWeight += WeightValue_t;
            }
            //_extendedShipmentService.RestartStopwatch(watch, $"بدست آوردن وزن کلی سفارش", ref Total);
            // int totalWeight = details.Cart.Sum(p => p.GetAttribute<string>("وزن دقیق").ToEnDigit());

            if (details.BillingAddress != null)
            {
                HagheMagharPrice = CheckAddressNeedHagheMaghar(details.BillingAddress, details.Customer.Id, serviceId, totalWeight);
                ShipmentHagheMaghar = getShipmentHagheMaghar(details.BillingAddress, details.Customer.Id, serviceId, ShipmentWeight);

            }
            //_extendedShipmentService.RestartStopwatch(watch, $"گرفتن حق مقر از دیتابیس", ref Total);

            if (HagheMagharPrice > 0)
            {
                if (details.OrderTotal > 0)
                {
                    details.OrderTotal += HagheMagharPrice + ShipmentHagheMaghar + (((HagheMagharPrice + ShipmentHagheMaghar) * 9) / 100);
                    details.OrderTaxTotal += ((HagheMagharPrice + ShipmentHagheMaghar * 9) / 100);
                }
                else if (details.RedeemedRewardPointsAmount > 0)
                {
                    details.RedeemedRewardPoints += (HagheMagharPrice + ShipmentHagheMaghar + (((HagheMagharPrice + ShipmentHagheMaghar) * 9) / 100));
                }
            }
            //_extendedShipmentService.RestartStopwatch(watch, $"محسابات حق مقر", ref Total);
            if (HagheMagharPrice > 0)
            {
                HagheMagharPrice += Convert.ToInt32(InTrafficAreaPrice);
            }
            return (HagheMagharPrice, ShipmentHagheMaghar);
        }
        public int CheckAddressNeedHagheMaghar(Address address, int customerId, int ServiceId, int TotalWeight
            , bool isInTarheTraffic = false, int InComeCount = 0)
        {
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "Int_CountryId", SqlDbType = SqlDbType.Int,Value = address.CountryId },
                new SqlParameter() { ParameterName = "Int_StateId", SqlDbType = SqlDbType.Int,Value = address.StateProvinceId },
                new SqlParameter() { ParameterName = "Nvc_Address", SqlDbType = SqlDbType.NVarChar,Value = address.Address1 },
                new SqlParameter() { ParameterName = "Nvc_FirstName", SqlDbType = SqlDbType.NVarChar,Value = address.FirstName },
                new SqlParameter() { ParameterName = "Nvc_LastName", SqlDbType = SqlDbType.NVarChar,Value = address.LastName },
                new SqlParameter() { ParameterName = "vc_PhoneNumber", SqlDbType = SqlDbType.NVarChar,Value = address.PhoneNumber },
                new SqlParameter() { ParameterName = "Int_CustomerId", SqlDbType = SqlDbType.Int, Value = customerId },
                new SqlParameter() { ParameterName = "ServiceId", SqlDbType = SqlDbType.Int, Value = ServiceId },
                new SqlParameter() { ParameterName = "InComeTotalWeight", SqlDbType = SqlDbType.Int, Value = TotalWeight },
                new SqlParameter() { ParameterName = "isInTarheTraffic", SqlDbType = SqlDbType.Bit, Value = isInTarheTraffic },
                //new SqlParameter() { ParameterName = "InComeCount", SqlDbType = SqlDbType.Int, Value = InComeCount}
            };
            int? price = _dbContext.SqlQuery<int>(@"EXECUTE [dbo].[Sp_CheckBillingAddressForHagheMaghar] @Int_CountryId,@Int_StateId,@Nvc_Address,@Nvc_FirstName,@Nvc_LastName,@vc_PhoneNumber
                                                    ,@Int_CustomerId,@ServiceId,@InComeTotalWeight,@isInTarheTraffic", prms).FirstOrDefault();
            return price.GetValueOrDefault(0);
        }


        public int getShipmentHagheMaghar(Address address, int customerId, int ServiceId, List<int> shipmentWeight)
        {
            if (DateTime.Now.CompareTo(Convert.ToDateTime("1399/07/23 12:00:01 PM")) < 0)
                return 0;
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "Int_CountryId", SqlDbType = SqlDbType.Int,Value = address.CountryId },
                new SqlParameter() { ParameterName = "Int_StateId", SqlDbType = SqlDbType.Int,Value = address.StateProvinceId },
                new SqlParameter() { ParameterName = "Nvc_Address", SqlDbType = SqlDbType.NVarChar,Value = address.Address1 },
                new SqlParameter() { ParameterName = "Nvc_FirstName", SqlDbType = SqlDbType.NVarChar,Value = address.FirstName },
                new SqlParameter() { ParameterName = "Nvc_LastName", SqlDbType = SqlDbType.NVarChar,Value = address.LastName },
                new SqlParameter() { ParameterName = "vc_PhoneNumber", SqlDbType = SqlDbType.NVarChar,Value = address.PhoneNumber },
                new SqlParameter() { ParameterName = "Int_CustomerId", SqlDbType = SqlDbType.Int, Value = customerId },
                new SqlParameter() { ParameterName = "ServiceId", SqlDbType = SqlDbType.Int, Value = ServiceId }
            };
            var Result = _dbContext.SqlQuery<ShipmentHagheMagharData>(@"EXECUTE [dbo].[Sp_GetShipmentDataForHagheMaghar] @Int_CountryId,@Int_StateId,@Nvc_Address,@Nvc_FirstName,@Nvc_LastName,@vc_PhoneNumber
                                                    ,@Int_CustomerId,@ServiceId", prms).ToList();
            int shouldCalc = 0;
            if (shipmentWeight.Any())
            {
                int weightSum = 0;
                int Counter = 0;
                bool IsOver30kg = false;
                foreach (var item in shipmentWeight)
                {
                    Counter++;
                    if (weightSum + item >= 20000)
                    {
                        if (Counter > 5)
                            shouldCalc += (Counter - 5);
                        weightSum = (weightSum + item) - 20000;
                        if (weightSum > 0)
                            Counter = 1;
                        else
                            Counter = 0;
                        IsOver30kg = true;
                    }
                    else
                    {
                        IsOver30kg = false;
                        weightSum += item;
                    }
                }
                if (!IsOver30kg)
                {
                    if (Counter >= 5)
                        shouldCalc += (Counter - 5);
                }
            }
            int SumOfShipmentHagheMaghr = 0;
            if (Result.Any())
            {
                int weightSum = 0;
                int Counter = 0;
                bool IsOver30kg = false;
                foreach (var item in Result)
                {
                    Counter++;
                    if (weightSum + item.ExactWeight >= 20000)
                    {
                        if (Counter > 5)
                            shouldCalc += (Counter - 5);
                        weightSum = (weightSum + item.ExactWeight) - 20000;
                        if (weightSum > 0)
                            Counter = 1;
                        else
                            Counter = 0;
                        IsOver30kg = true;
                    }
                    else
                    {
                        IsOver30kg = false;
                        weightSum += item.ExactWeight;
                    }
                }
                if (!IsOver30kg)
                {
                    if (Counter > 5)
                        shouldCalc += (Counter - 5);
                }
                SumOfShipmentHagheMaghr = Result.Sum(p => p.ShipmentHagheMaghr);
            }
            int shipmentHagheMagar = (shouldCalc * 13000) - SumOfShipmentHagheMaghr;
            if (shipmentHagheMagar < 0)
                return 0;
            return shipmentHagheMagar;
        }


        public class ShipmentHagheMagharData
        {
            public int ShipmentId { get; set; }
            public int ExactWeight { get; set; }
            public int HagheMagharPrice { get; set; }
            public int ShipmentHagheMaghr { get; set; }
        }

        protected Order SaveOrderShipments(PlaceOrderContainer details, Order order, List<ExnShippmentModel> shipments = null)
        {
            if (shipments == null)
            {
                shipments = new List<ExnShippmentModel>() {
                    new ExnShippmentModel() { }
                };
            }
            if (shipments.Count == 1)
            {
                var XtnShipment = shipments.FirstOrDefault();
                var shipment = XtnShipment.shipment;
                shipment.OrderId = order.Id;
                shipment.CreatedOnUtc = (DateTime.UtcNow);
                shipment.ShipmentItems.Clear();
                foreach (var item in order.OrderItems)
                {
                    shipment.ShipmentItems.Add(new ShipmentItem() { OrderItemId = item.Id, Quantity = 1 /*item.Quantity*/ });
                }
                _shipmentService.InsertShipment(shipment);
                XtnShipment.ShipmentId = shipment.Id;
                _exnShippmentRepository.Insert(XtnShipment);
            }
            else if (shipments.Count > 1)
            {
                foreach (var XtnShipment in shipments)
                {
                    Shipment shipment = XtnShipment.shipment;
                    shipment.OrderId = order.Id;
                    shipment.CreatedOnUtc = (DateTime.UtcNow);
                    foreach (var shipmentItem in shipment.ShipmentItems)
                    {
                        var cartItem = details.Cart.FirstOrDefault(p => p.Id == shipmentItem.OrderItemId);
                        shipmentItem.OrderItemId = order.OrderItems.FirstOrDefault(p => p.ProductId.Equals(cartItem.ProductId) && p.AttributesXml == cartItem.AttributesXml).Id;
                    }
                    _shipmentService.InsertShipment(shipment);
                    XtnShipment.ShipmentId = shipment.Id;
                    _exnShippmentRepository.Insert(XtnShipment);
                }
            }
            _genericAttributeService.SaveAttribute<string>(order.Customer, "IsOrderMultishipment_" + order.Id,
            "true", order.StoreId);
            return order;
        }
        /// <summary>
        /// check first order and add tag
        /// </summary>
        /// <param name="details">Place order container</param>
        /// <param name="order">Order</param>
        protected bool CheckFirstOrder(Order order)
        {
            var customerOrderCount = _extendedShipmentService.GetOrdersByCustomerId(order.CustomerId);
            if (customerOrderCount == 0)
            {
                FirstOrderModel model = new FirstOrderModel { OrderId = order.Id };
                _firstOrdeRepository.Insert(model);
                return true;
            }
            return false;
        }

        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<Nop.Services.Logging.ILogger>();
            logger.InsertLog(Nop.Core.Domain
                .Logging.LogLevel.Information, header, Message, null);
        }
        public List<AddressModel> getOrderAddress(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (IsMultiShippment(order))
            {
                var AddressIds = this._dbContext.SqlQuery<int>(
                String.Format(@"SELECT A.Id FROM 
                dbo.Shipment S 
                INNER JOIN dbo.ShipmentItem SI ON SI.ShipmentId = S.Id INNER JOIN dbo.OrderItem OI ON OI.Id = SI.OrderItemId
                INNER JOIN dbo.XtnShippment XS ON XS.ShipmentId = S.Id INNER JOIN dbo.Address A ON A.Id = xs.ShippmentAddressId
                WHERE S.orderId = {0} ", orderId), new object[0]).ToList();
                var addressModel = new List<AddressModel>();
                foreach (var item in AddressIds)
                {
                    var address = _addressService.GetAddressById(item).ToModel();
                    preperAddressModel(address);
                    addressModel.Add(address);
                }
                return addressModel;
            }
            else
            {
                var address = order.ShippingAddress.ToModel();
                preperAddressModel(address);
                return new List<AddressModel>() { address };
            }
        }
        public AddressModel getShippingAddres(int shipmentId)
        {
            var AddressIds = this._dbContext.SqlQuery<int>(
               String.Format(@"SELECT A.Id FROM 
                dbo.Shipment S 
                INNER JOIN dbo.ShipmentItem SI ON SI.ShipmentId = S.Id INNER JOIN dbo.OrderItem OI ON OI.Id = SI.OrderItemId
                INNER JOIN dbo.XtnShippment XS ON XS.ShipmentId = S.Id INNER JOIN dbo.Address A ON A.Id = xs.ShippmentAddressId
                WHERE S.Id = {0} ", shipmentId), new object[0]).FirstOrDefault();
            var addressModel = new AddressModel();
            if (AddressIds == 0)
            {
                addressModel = _shipmentService.GetShipmentById(shipmentId).Order?.ShippingAddress.ToModel();
            }
            else
            {
                addressModel = _addressService.GetAddressById(AddressIds).ToModel();
            }

            if (addressModel == null)
                return null;
            preperAddressModel(addressModel);
            return addressModel;
        }
        public bool IsMultiShippment(Order order)
        {
            var str_IsMultiShippment = order.Customer.GetAttribute<string>("IsOrderMultishipment_" + order.Id, _genericAttributeService
                , order.StoreId);
            if (string.IsNullOrEmpty(str_IsMultiShippment))
                return false;
            return true;
        }
        public bool IsMultiShippment(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var str_IsMultiShippment = order.Customer.GetAttribute<string>("IsOrderMultishipment_" + order.Id, _genericAttributeService
                , order.StoreId);
            if (string.IsNullOrEmpty(str_IsMultiShippment))
                return false;
            return true;
        }
        private void preperAddressModel(AddressModel model)
        {
            model.CountryEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.StateProvinceEnabled = true;
            model.CityEnabled = true;
            model.StreetAddress2Enabled = true;
            model.StreetAddressEnabled = true;
            model.CompanyEnabled = true;
            model.FaxEnabled = true;
            model.PhoneEnabled = true;
            model.EmailEnabled = true;
            model.FirstNameEnabled = true;
            model.LastNameEnabled = true;
        }
        public List<OrderDetails_ShippingModel> Get_OrderDetails_Shipping(int orderId, int pageSize, int PageIndex, out int count)
        {
            SqlParameter P_Count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Value = pageSize, Direction = ParameterDirection.Output };
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "OrderId", SqlDbType = SqlDbType.Int,Value = orderId },
                new SqlParameter() { ParameterName = "pageIndex", SqlDbType = SqlDbType.Int,Value =  PageIndex },
                new SqlParameter() { ParameterName = "pageSize", SqlDbType = SqlDbType.Int,Value = pageSize},
                P_Count
            };
            var Data = _dbContext.SqlQuery<OrderDetails_ShippingModel>(@"EXECUTE [dbo].[Sp_OrderDetails_Shipping] @OrderId,@pageIndex,@pageSize,@Count out", prms).ToList();
            count = (int)P_Count.Value;
            return Data;
        }
        /// <summary>
        /// بازگشت وجه به کیف پول مشتری
        /// </summary>
        /// <param name="order"></param>

    }
    public class OrderDetails_ShippingModel
    {
        public int OrderId { get; set; }
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public DateTime? DataCollect { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Address { get; set; }
        public int ExactWeight { get; set; }
        public string TrackingNumber { get; set; }
        public bool PackingPurchased { get; set; }
        public string CollectorCustomerName { get; set; }
        public string DistributerCustomerName { get; set; }
    }
}
