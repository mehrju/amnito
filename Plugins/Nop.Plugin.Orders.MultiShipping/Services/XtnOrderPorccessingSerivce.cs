using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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


namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class XtnOrderProcessingService : OrderProcessingService
    {
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;
        private readonly IRepository<FirstOrderModel> _firstOrderRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IDbContext _dbContext;
        private readonly IHagheMaghar _hagheMaghar;
        private readonly IStoreContext _storeContext;
        private readonly IRewardPointService _rewardPointService;
        private readonly ISettingService _settingService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IRepository<Tbl_CancelReason_Order> _repositoryTbl_CancelReason_Order;


        public
            XtnOrderProcessingService(IAddressService addressService, IStoreContext storeContext, IHagheMaghar hagheMaghar, IOrderService orderService, IWebHelper webHelper,
                ILocalizationService localizationService, ILanguageService languageService,
                IProductService productService, IPaymentService paymentService, ILogger logger,
                IOrderTotalCalculationService orderTotalCalculationService,
                IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
                IProductAttributeParser productAttributeParser, IProductAttributeFormatter productAttributeFormatter,
                IGiftCardService giftCardService, IShoppingCartService shoppingCartService,
                ICheckoutAttributeFormatter checkoutAttributeFormatter, IShippingService shippingService,
                IShipmentService shipmentService, ITaxService taxService, ICustomerService customerService,
                IDiscountService discountService, IEncryptionService encryptionService, IWorkContext workContext,
                IWorkflowMessageService workflowMessageService, IVendorService vendorService,
                ICustomerActivityService customerActivityService, ICurrencyService currencyService,
                IAffiliateService affiliateService, IEventPublisher eventPublisher, IPdfService pdfService,
                IRewardPointService rewardPointService, IGenericAttributeService genericAttributeService,
                ICountryService countryService, IStateProvinceService stateProvinceService,
                ShippingSettings shippingSettings, PaymentSettings paymentSettings,
                RewardPointsSettings rewardPointsSettings, OrderSettings orderSettings, TaxSettings taxSettings,
                LocalizationSettings localizationSettings, CurrencySettings currencySettings,
                ICustomNumberFormatter customNumberFormatter, IRepository<FirstOrderModel> firstOrderRepository
                , IRepository<Order> orderRepository, IDbContext dbContext,
                ISettingService settingService, IExtendedShipmentService extendedShipmentService,
                IRepository<Tbl_CancelReason_Order> repositoryTbl_CancelReason_Order) : base(orderService, webHelper, localizationService,
            languageService, productService, paymentService, logger, orderTotalCalculationService,
            priceCalculationService, priceFormatter, productAttributeParser, productAttributeFormatter, giftCardService,
            shoppingCartService, checkoutAttributeFormatter, shippingService, shipmentService, taxService,
            customerService, discountService, encryptionService, workContext, workflowMessageService, vendorService,
            customerActivityService, currencyService, affiliateService, eventPublisher, pdfService, rewardPointService,
            genericAttributeService, countryService, stateProvinceService, shippingSettings, paymentSettings,
            rewardPointsSettings, orderSettings, taxSettings, localizationSettings, currencySettings,
            customNumberFormatter)
        {
            _extendedShipmentService = extendedShipmentService;
            _repositoryTbl_CancelReason_Order = repositoryTbl_CancelReason_Order;
            _addressService = addressService;
            _hagheMaghar = hagheMaghar;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _logger = logger;
            _firstOrderRepository = firstOrderRepository;
            _orderRepository = orderRepository;
            _dbContext = dbContext;
            _storeContext = storeContext;
            _rewardPointService = rewardPointService;
            _settingService = settingService;
            _priceCalculationService = priceCalculationService;
            _taxService = taxService;
            _productAttributeFormatter = productAttributeFormatter;
            _shippingService = shippingService;
            _orderService = orderService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _productAttributeParser = productAttributeParser;
            _genericAttributeService = genericAttributeService;
        }
        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        public override PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);

            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();
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
                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);
                if (IsOrderAddressLikeness(details))
                {
                    return new PlaceOrderResult()
                    {
                        Errors = new List<string>() { "آدرس فرستنده و گیرنده نمی توانند یکسان باشند" },
                        PlacedOrder = null
                    };
                }
                #region HagheMaghar

                int totalWeight = 0;
                foreach (var item in details.Cart)
                {
                    var PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                    var weight = PAM.Where(p => p.ProductAttribute.Name == "وزن دقیق").FirstOrDefault();
                    if (weight != null)
                    {
                        var WeightValue = _productAttributeParser.ParseValues(item.AttributesXml, weight.Id).FirstOrDefault();
                        if (!string.IsNullOrEmpty(WeightValue))
                            totalWeight += WeightValue.ToEnDigit() * item.Quantity;
                    }
                }
                var HagheMagharPrice = CalcHagheMaghar(details, processPaymentRequest, totalWeight);
                #endregion

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

                bool HasRepresentativeServices = CalcAgentSaleAmount(details, customer);

                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);
                    if (HagheMagharPrice > 0)
                    {
                        _hagheMaghar.Insert(order.OrderItems.First().Id, HagheMagharPrice, 0);
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

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id);

                    //check order status
                    CheckOrderStatus(order);
                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));

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
            // var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            _logger.Error(logError, customer: customer);

            return result;
        }

        private bool IsOrderAddressLikeness(PlaceOrderContainer details)
        {
            if (details.BillingAddress == null || details.ShippingAddress == null)
                return false;
            return (details.BillingAddress.Country == details.ShippingAddress.Country
                && details.BillingAddress.StateProvince == details.ShippingAddress.StateProvince
                && details.BillingAddress.Address1 == details.ShippingAddress.Address1);
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
                    AgentSaleAmount += txtPrice.ToEnDigit();
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
        private int CalcHagheMaghar(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest, int TotalWeight)
        {
            int HagheMagharPrice = 0;
            var setting =
                _settingService.GetSetting("NopMaster.Wallet_ProductId", _storeContext.CurrentStore.Id, false);
            int productId = setting == null ? 0 : int.Parse(setting.Value);
            if (productId != 0)
                if (details.Cart.Any(p => p.ProductId == productId))
                    return 0;

            int serviceId = details.Cart.SelectMany(p => p.Product.ProductCategories.Select(d => d.CategoryId)).First();
            if (details.BillingAddress != null && processPaymentRequest.PaymentMethodSystemName != "Payments.CashOnDelivery")
                HagheMagharPrice = checkBillingAddress(details.BillingAddress, details.Customer.Id, serviceId, TotalWeight);
            if (HagheMagharPrice > 0)
            {
                if (details.OrderTotal > 0)
                {
                    details.OrderTotal += HagheMagharPrice + ((HagheMagharPrice * 9) / 100);
                    details.OrderTaxTotal += ((HagheMagharPrice * 9) / 100);
                }
                else if (details.RedeemedRewardPointsAmount > 0)
                {
                    details.RedeemedRewardPoints += (HagheMagharPrice + ((HagheMagharPrice * 9) / 100));

                }
            }

            return HagheMagharPrice;
        }
        private void MoveShoppingCartItemsToOrderItems(PlaceOrderContainer details, Order order, int AgentSAleAwount)
        {
            foreach (var sc in details.Cart)
            {
                //prices
                var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out decimal discountAmount,
                    out List<DiscountForCaching> scDiscounts, out int? _);
                var scUnitPriceInclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out decimal _);
                var scUnitPriceExclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out _);
                var scSubTotalInclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out _);
                var scSubTotalExclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out _);
                var discountAmountInclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, true, details.Customer, out _);
                var discountAmountExclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, false, details.Customer, out _);
                foreach (var disc in scDiscounts)
                    if (!details.AppliedDiscounts.ContainsDiscount(disc))
                        details.AppliedDiscounts.Add(disc);

                //attributes
                var attributeDescription =
                    _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

                var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);

                //save order item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    Order = order,
                    ProductId = sc.ProductId,
                    UnitPriceInclTax = scUnitPriceInclTax,
                    UnitPriceExclTax = scUnitPriceExclTax,
                    PriceInclTax = scSubTotalInclTax,
                    PriceExclTax = scSubTotalExclTax,
                    OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = sc.AttributesXml,
                    Quantity = sc.Quantity,
                    DiscountAmountInclTax = discountAmountInclTax,
                    DiscountAmountExclTax = discountAmountExclTax,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = sc.RentalStartDateUtc,
                    RentalEndDateUtc = sc.RentalEndDateUtc
                };
                order.OrderItems.Add(orderItem);
                _orderService.UpdateOrder(order);

                //gift cards
                AddGiftCards(sc.Product, sc.AttributesXml, sc.Quantity, orderItem, scUnitPriceExclTax);

                //inventory
                _productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.PlaceOrder"),
                        order.Id));
            }

            //clear shopping cart

        }
        protected void CheckFirstOrder(Order order)
        {
            var customerOrderCount = GetOrdersByCustomerId(order.CustomerId);
            if (customerOrderCount != 1) return;
            FirstOrderModel model = new FirstOrderModel { OrderId = order.Id };
            _firstOrderRepository.Insert(model);
        }
        protected int GetOrdersByCustomerId(int customerId)
        {
            if (customerId == 0)
                return -1;
            return _orderRepository.Table.Count(p => p.CustomerId == customerId);

        }

        public int checkBillingAddress(Address address, int customerId, int servicId, int TotalWeight, bool isInTarheTraffic = false)
        {
            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter() { ParameterName = "@Int_CountryId", SqlDbType = SqlDbType.Int,Value = address.CountryId },
                new SqlParameter() { ParameterName = "@Int_StateId", SqlDbType = SqlDbType.Int,Value = address.StateProvinceId },
                new SqlParameter() { ParameterName = "@Nvc_Address", SqlDbType = SqlDbType.NVarChar,Value = address.Address1 },
                new SqlParameter() { ParameterName = "@Nvc_FirstName", SqlDbType = SqlDbType.NVarChar,Value = address.FirstName },
                new SqlParameter() { ParameterName = "@Nvc_LastName", SqlDbType = SqlDbType.NVarChar,Value = address.LastName },
                new SqlParameter() { ParameterName = "@vc_PhoneNumber", SqlDbType = SqlDbType.NVarChar,Value = address.PhoneNumber },
                new SqlParameter() { ParameterName = "Int_CustomerId", SqlDbType = SqlDbType.Int, Value = customerId },
                new SqlParameter() { ParameterName = "ServiceId", SqlDbType = SqlDbType.Int, Value = servicId },
                new SqlParameter() { ParameterName = "InComeTotalWeight", SqlDbType = SqlDbType.Int, Value = TotalWeight },
                new SqlParameter() { ParameterName = "isInTarheTraffic", SqlDbType = SqlDbType.Bit, Value = isInTarheTraffic }
            };
            int? price = _dbContext.SqlQuery<int>(@"EXECUTE [dbo].[Sp_CheckBillingAddressForHagheMaghar] @Int_CountryId,@Int_StateId,@Nvc_Address,@Nvc_FirstName,@Nvc_LastName,@vc_PhoneNumber,@Int_CustomerId,@ServiceId,@InComeTotalWeight,@isInTarheTraffic", prms).FirstOrDefault();
            return price.GetValueOrDefault(0);
        }

        public override void CancelOrder(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCancelOrder(order))
                throw new NopException($"امکان کنسل کردن سفارش شماره {order.Id} وجود ندارد");

            if (order.OrderItems.Any(x => x.Product.ProductCategories.Any(y => y.CategoryId == 730 || y.CategoryId == 731)))//mahex
            {
                // _extendedShipmentService.MakeMahexVoid(order.Shipments.First().TrackingNumber);
            }
            if (new int[] { 730, 731 }.Contains(order.OrderItems.First().Product.ProductCategories.First().CategoryId))
            {
                foreach (var item in order.Shipments)
                {
                    if (!string.IsNullOrEmpty(item.TrackingNumber))
                        _extendedShipmentService.MakeMahexVoid(item.TrackingNumber);
                }
            }
            //cancel order
            SetOrderStatus(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            AddOrderNote(order, $"سفارش شماره {order.Id} کنسل شد");

            //return (add) back redeemded reward points
            ReturnBackRedeemedRewardPoints(order);

            //cancel recurring payments
            var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);
            foreach (var rp in recurringPayments)
            {
                CancelRecurringPayment(rp);
            }

            //Adjust inventory for already shipped shipments
            //only products with "use multiple warehouses"
            foreach (var shipment in order.Shipments)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                        string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
                }
            }
            //Adjust inventory
            foreach (var orderItem in order.OrderItems)
            {
                _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
            }

            _eventPublisher.Publish(new OrderCancelledEvent(order));
        }
        public override bool CanCancelOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;
            if (order.Shipments.Any(p => p.ShippedDateUtc != null || p.DeliveryDateUtc != null))
                return false;
            return true;
        }

        protected override void ReturnBackRedeemedRewardPoints(Order order)
        {
            int RefoundedValue = 0;
            if (_repositoryTbl_CancelReason_Order.Table.Any(p => p.OrderId == order.Id))
            {
                string RefoundItem = _repositoryTbl_CancelReason_Order.Table.First(p => p.OrderId == order.Id).RefoundItem;
                if (!string.IsNullOrEmpty(RefoundItem))
                {
                    var refoundItemsObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CancelOrderCostInfo>>(RefoundItem);
                    RefoundedValue = refoundItemsObj.Sum(p => p.EnteredValue + p.CalculatedTax);
                }
                if (RefoundedValue > 0)
                {
                    _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, RefoundedValue, order.StoreId,
                   string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
                    _orderService.UpdateOrder(order);
                }
                //base.ReturnBackRedeemedRewardPoints(order);
                return;
            }
            if (order.RedeemedRewardPointsEntry == null)
            {
                //base.ReturnBackRedeemedRewardPoints(order);
                return;
            }

            //return back
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -order.RedeemedRewardPointsEntry.Points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
            _orderService.UpdateOrder(order);
            //base.ReturnBackRedeemedRewardPoints(order);
        }

    }
}