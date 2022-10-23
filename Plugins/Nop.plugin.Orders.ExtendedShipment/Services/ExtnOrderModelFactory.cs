using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ExtnOrderModelFactory : OrderModelFactory,IExtnOrderModelFactory
    {
        #region Fields
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;

        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly IProductService _productService;
        #endregion
        #region ctor
        public ExtnOrderModelFactory(IExtendedShipmentService extendedShipmentService,
           IAddressModelFactory addressModelFactory,
           IOrderService orderService,
           IWorkContext workContext,
           ICurrencyService currencyService,
           IPriceFormatter priceFormatter,
           IOrderProcessingService orderProcessingService,
           IDateTimeHelper dateTimeHelper,
           IPaymentService paymentService,
           ILocalizationService localizationService,
           IShippingService shippingService,
           ICountryService countryService,
           IProductAttributeParser productAttributeParser,
           IDownloadService downloadService,
           IStoreContext storeContext,
           IOrderTotalCalculationService orderTotalCalculationService,
           IRewardPointService rewardPointService,
           CatalogSettings catalogSettings,
           OrderSettings orderSettings,
           TaxSettings taxSettings,
           ShippingSettings shippingSettings,
           AddressSettings addressSettings,
           RewardPointsSettings rewardPointsSettings,
           PdfSettings pdfSettings,
           IProductService productService) : base(addressModelFactory,
            orderService,
            workContext,
            currencyService,
            priceFormatter,
            orderProcessingService,
            dateTimeHelper,
            paymentService,
            localizationService,
            shippingService,
            countryService,
            productAttributeParser,
            downloadService,
            storeContext,
            orderTotalCalculationService,
            rewardPointService,
            catalogSettings,
            orderSettings,
            taxSettings,
            shippingSettings,
            addressSettings,
            rewardPointsSettings,
            pdfSettings)
        {
            this._productService = productService;
            this._extendedShipmentService = extendedShipmentService;
            this._addressModelFactory = addressModelFactory;
            this._orderService = orderService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;

            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
        } 
        #endregion
        public CustomerOrderListModel ExnPrepareCustomerOrderListModel(int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue,
           int SenderStateProvinceId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null,List<int> OrderStatusIds=null
            , List<int> paymentStatusIds = null, List<int> shippingStatusIds = null, int ProductId = 0,
           int OrderId = 0)
        {
            var model = new CustomerOrderListModel();
            var filterByProductId = 0;
            var product = _productService.GetProductById(ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = productId;
            int count = 0;
            Models.CoardinationStatisticModel CoardinationStatistic = null;
            var orders = _extendedShipmentService.SearchOrders(out CoardinationStatistic,out count,storeId: storeId,
               vendorId: vendorId,
               customerId: customerId,
               OrderId: OrderId,
               productId: filterByProductId,
               warehouseId: warehouseId,
               paymentMethodSystemName: paymentMethodSystemName,
               createdFromUtc: createdFromUtc,
               createdToUtc: createdToUtc,
               osIds: OrderStatusIds,
               psIds: paymentStatusIds,
               ssIds: shippingStatusIds,
               billingEmail: billingEmail,
               billingLastName: billingLastName,
               billingCountryId: billingCountryId,
               orderNotes: orderNotes,
               pageIndex: 0,
               pageSize: 50000,
               SenderStateProvinceId: SenderStateProvinceId,
               ReciverCountryId: ReciverCountryId,
               ReciverStateProvinceId: ReciverStateProvinceId,
               ReciverName: ReciverName,
               SenderName: SenderName);
            foreach (var item in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel
                {
                    Id = item.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(item.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = item.OrderStatus,
                    OrderStatus = item.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = item.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = item.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(item),
                    CustomOrderNumber = item.CustomOrderNumber
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(item.OrderTotal, item.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, item.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.Orders.Add(orderModel);
            }

            var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = $"{recurringPayment.CycleLength} {recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)}",
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    InitialOrderNumber = recurringPayment.InitialOrder.CustomOrderNumber,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    CanRetryLastPayment = _orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            return model;
        }
        protected bool HasAccessToProduct(Nop.Core.Domain.Catalog.Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return product.VendorId == vendorId;
        }
    }
}
