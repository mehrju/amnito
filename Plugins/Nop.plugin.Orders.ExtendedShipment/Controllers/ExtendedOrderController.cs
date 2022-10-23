using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Tools;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ExtendedOrderController : Nop.Web.Controllers.OrderController
    {
        #region Fields
        private readonly ICollectorService _collectorService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ICodService _codService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<StateCodemodel> _repositoryStateCode;
        private readonly IRepository<CountryCodeModel> _repositoryCountryCode;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IShippingService _shippingService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly ICountryService _countryService;
        private readonly ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IShipmentService _shipmentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderReportService _orderReportService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly IExtnOrderModelFactory _extnOrderModelFactory;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IDeclaration_Status_foreign_order_Service _declaration_Status_Foreign_Order_Service;
        private readonly IRelatedOrders_Service _tb_RelatedOrders_Service;
        private readonly ICalcPriceOrderItem_Service _calcPriceOrderItem_Service;
        private readonly ITrackingUbaarOrder_Service _trackingUbaarOrder_Service;
        private readonly IPhoneOrderService _phoneOrderService;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        private readonly IRepository<ShipmentEventModel> _repository_ShipmentEvent;
        private readonly IRepository<Category> _repository_Category;
        private readonly IRepository<Tbl_ShipmentEventCategory> _repository_ShipmentEventCategory;
        private readonly IRewardPointCashoutService _rewardPointCashoutService;

        //private readonly IRepository<RelatedOrders> _repositoryTb_RelatedOrders;

        #endregion

        #region ctor
        public ExtendedOrderController(
            ICollectorService collectorService,
            IStoreContext storeContext,
            ICustomerService customerService,
            ICodService codService,
            INotificationService notificationService,
            ICustomerActivityService customerActivityService,
            IPaymentService paymentService,
            IOrderProcessingService orderProcessingService,
            IPdfService pdfService,
            IWebHelper webHelper,
            RewardPointsSettings rewardPointsSettings,
            IOrderReportService orderReportService,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IProductService productService,
            IExtendedShipmentService extendedShipmentService,
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService,
            IOrderService orderService,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            IShippingService shippingService,
            IProductAttributeParser productAttributeParser,
            ILocalizationService localizationService,
            ICountryService countryService,
            ShippingSettings shippingSettings,
            ISettingService settingService,
            IShipmentService shipmentService,
            IStoreService storeService,
            IPriceFormatter priceFormatter,
            IExtnOrderModelFactory extnOrderModelFactory,
            IOrderModelFactory orderModelFactory,
            IRepository<StateCodemodel> repositoryStateCode,
            IRepository<CountryCodeModel> repositoryCountryCode,
            IDeclaration_Status_foreign_order_Service declaration_Status_Foreign_Order_Service,
            IRelatedOrders_Service tb_RelatedOrders_Service,
            ICalcPriceOrderItem_Service calcPriceOrderItem_Service,
            ITrackingUbaarOrder_Service trackingUbaarOrder_Service,
            IPhoneOrderService phoneOrderService,
            IShipmentTrackingService shipmentTrackingService,
            IRewardPointCashoutService rewardPointCashoutService,
            IRepository<ShipmentEventModel> repository_shipmentEvent,
            IRepository<Category> repository_Category,
            IRepository<Tbl_ShipmentEventCategory> repository_ShipmentEventCategory
            ) : base(orderModelFactory,
             orderService,
             shipmentService,
             workContext,
             orderProcessingService,
             paymentService,
             pdfService,
             webHelper,
             rewardPointsSettings)
        {
            _collectorService = collectorService;
            _storeContext = storeContext;
            _customerService = customerService;
            _orderProcessingService = orderProcessingService;
            this._rewardPointCashoutService = rewardPointCashoutService;
            this._customerActivityService = customerActivityService;
            this._paymentService = paymentService;
            this._orderModelFactory = orderModelFactory;
            this._extnOrderModelFactory = extnOrderModelFactory;
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._productService = productService;
            this._storeService = storeService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._measureService = measureService;
            this._orderService = orderService;
            this._extendedShipmentService = extendedShipmentService;
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._measureSettings = measureSettings;
            this._shippingService = shippingService;
            this._localizationService = localizationService;
            this._shippingSettings = shippingSettings;
            this._settingService = settingService;
            this._shipmentService = shipmentService;
            this._priceFormatter = priceFormatter;
            this._orderReportService = orderReportService;
            this._repositoryCountryCode = repositoryCountryCode;
            this._repositoryStateCode = repositoryStateCode;
            this._codService = codService;
            this._notificationService = notificationService;
            this._declaration_Status_Foreign_Order_Service = declaration_Status_Foreign_Order_Service;
            this._tb_RelatedOrders_Service = tb_RelatedOrders_Service;
            this._calcPriceOrderItem_Service = calcPriceOrderItem_Service;
            this._trackingUbaarOrder_Service = trackingUbaarOrder_Service;
            _phoneOrderService = phoneOrderService;
            _shipmentTrackingService = shipmentTrackingService;
            _repository_ShipmentEvent = repository_shipmentEvent;
            _repository_Category = repository_Category;
            _repository_ShipmentEventCategory = repository_ShipmentEventCategory;
        }
        #endregion

        #region Admin Area

        //[HttpPost]
        //[Area(AreaNames.Admin)]
        //public IActionResult OrderStateStatistic(int state,int treeCountryId,int treeStateId,int treeCustomerId)
        //{
        //    var data = _extendedShipmentService.GetOrderStateStatistic(state, _workContext.CurrentCustomer.Id);
        //    return Json(data);
        //}

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult CancelCodOrder(string trackingNumber)
        {
            string result = "";
            bool resultOf = _codService.ChangeStatus(1, trackingNumber, out result);
            return Json(new { success = resultOf, message = result });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult savePostCord(int[] ordersId, string Desc, bool sendSmsToPostAdmin = true)
        {
            _extendedShipmentService.SavePostCoordination(ordersId.ToList(), Desc);
            string orderIds = "";
            foreach (var item in ordersId)
            {
                var order = _orderService.GetOrderById(item);
                int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;

                if (!order.OrderNotes.Any(p => p.Note.Contains("SmsNewOrderSend")))
                {
                    _extendedShipmentService.InsertOrderNote("SmsNewOrderSend", order.Id);
                    orderIds += orderIds == "" ? "" : "," + item;
                }
                if (_extendedShipmentService.IsPostService(serviceId))
                {
                    if (sendSmsToPostAdmin)
                    {
                        if (!order.OrderNotes.Any(p => p.Note.Contains("SendSmsSupervisor")))
                        {
                            _extendedShipmentService.InsertOrderNote("SendSmsSupervisor", order.Id);
                            //_notificationService.NotifyPostSupervisor(order, serviceId, _extendedShipmentService);
                        }
                    }
                }
            }
            if (orderIds != "")
            {
                try
                {
                    _notificationService.sendSmsPostAdminForNewOrder(orderIds, _extendedShipmentService);
                }
                catch (Exception ex)
                {
                    _extendedShipmentService.Log("خطا در زمان ارسال پیامک سفارش جدید برای مدیر و ناظر پست ", ex.Message +
                                                                                                             (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }
            SuccessNotification("عملیات با موفقیت انجام شد");
            return Json(new { result = true });
        }

        [Area(AreaNames.Admin)]
        public IActionResult NewShipmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new ShipmentListModel();
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(model);
        }


        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public void CleanToSendDataToPostAgain(int[] ordersId)
        {
            if (ordersId == null)
                return;
            _extendedShipmentService.CleanToSendDataToPostAgain(ordersId.ToList());
            SuccessNotification("عملیات با موفقیت انجام شد");
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult ChangeOrdersStatus(int[] ordersId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            List<string> Lst_Error = new List<string>();
            foreach (var id in ordersId)
            {
                var order = _orderService.GetOrderById(id);
                if (order == null)
                    //No order found with the specified id
                    return RedirectToAction("List");
                if (order.OrderStatus == OrderStatus.Cancelled)
                    continue;
                try
                {
                    order.OrderStatusId = 30;
                    _orderService.UpdateOrder(order);
                    //add a note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = $"وضعیت سفارش ویرایش شد. وضعیت جدید : {OrderStatus.Complete.GetLocalizedEnum(_localizationService, _workContext)}",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);
                    LogEditOrder(order.Id);
                }
                catch (Exception ex)
                {
                    Lst_Error.Add("خطا در زمان تکمیل کردن سفارش" + order.Id);
                    _extendedShipmentService.Log("خطا در زمان تکمیل کردن سفارش" + order.Id, ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "");

                }
            }
            if (Lst_Error.Any())
            {
                ErrorNotification("عملیات انجام شد " + "\r\n" + string.Join("\r\n", Lst_Error), false);
                return Json(new { result = 1 });
            }
            SuccessNotification("عملیات با موفقیت انجام شد");
            return Json(new { result = 0 });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult getOrderOverView(int orderState)
        {
            return Json(_extendedShipmentService.OrdersOverView(orderState));
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult getShipmentOverView(int orderState)
        {
            return Json(_extendedShipmentService.ShipmentTreeView(orderState));
        }

        public IActionResult GetOrderStatus()
        {
            return Ok(_repository_ShipmentEvent.TableNoTracking.Select(p => new { Text = p.ShipmentEventName, Value = p.ShipmentEventId }).ToList());
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult getShipmentOverViewByStatus(int orderState, OrderShipmentStatusEnum shipmentStatusEnum, int categoryId)
        {
            return Json(_extendedShipmentService.ShipmentTreeViewByStatus(orderState, shipmentStatusEnum, categoryId));
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        public IActionResult OrderList(DataSourceRequest command, ExtendedOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();
            if (_workContext.CurrentCustomer.CustomerRoles.Any(p => p.Id == 10))
            {
                return AccessDeniedKendoGridJson();
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            bool LimitDate = (model.treeCountryId > 0 || model.treeCustomerId > 0 || model.treeStateId > 0) && !(new int[] { 11, 12, 13, 14, 15 }.Contains(model.orderState));
            var startDateValue = LimitDate ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(DateTime.Now).AddDays(-10) :
                                model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            int count = 0;
            CoardinationStatisticModel _CoardinationStatistic = null;
            var orders = _extendedShipmentService.SearchOrders(out _CoardinationStatistic, out count, storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.treeCountryId > 0 ? model.treeCountryId : model.BillingCountryId,
                orderNotes: model.OrderNotes,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                SenderStateProvinceId: model.treeStateId > 0 ? model.treeStateId : model.SenderStateProvinceId,
                ReciverCountryId: model.ReciverCountryId,
                ReciverStateProvinceId: model.ReciverStateProvinceId,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                IsOrderOutDate: model.IsOrderOutDate,
                orderState: model.orderState,
                customerId: model.treeCustomerId
                );
            //_extendedShipmentService.Log(orders.Count(p => p.PaymentMethodSystemName == "Payments.CashOnDelivery") + "تعداد COD", "");
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new
                    {
                        Id = x.Id,
                        StoreName = SumProductName(x.OrderItems) + (x.IsInternalForForeign ? "-IFX" : ""),// store != null ? store.Name : "Unknown",
                        OrderTotal = ((x.OrderStatusId != 20 && x.OrderStatusId != 30 && x.PaymentMethodSystemName == null) ? "" : _priceFormatter.FormatPrice((x.OrderTotal == 0 && x.PaymentMethodSystemName == null ? (x.RedeemedRewardPointsEntry == null ? 0 : x.RedeemedRewardPointsEntry.Points * -1) : x.OrderTotal), true, false)),
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        OrderStatusId = x.OrderStatusId,
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatusId = x.PaymentStatusId,
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatusId = x.ShippingStatusId,
                        CustomerEmail = x.BillingAddress.Email,
                        CustomerFullName = $"{x.BillingAddress.FirstName} {x.BillingAddress.LastName}",
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomOrderNumber = x.CustomOrderNumber,
                        PaymentMethodSystemName = x.PaymentMethodSystemName,
                        CoordinationDate = x.CoordinationDate,
                        Desc = x.Desc,
                        IsFirstORder = x.IsFirstORder,
                        NeedPrinter = x.NeedPrinter,
                        NeedCarton = x.NeedCarton,
                        IsUbaar = x.IsUbaar,
                        x.CartonSizeName,
                        x.IsInternalForForeign
                    };
                }),
                Total = count
                ,
            };

            //summary report
            //currently we do not support productId and warehouseId parameters for this report
            var reportSummary = _extendedShipmentService.GetOrderAverageReportLine(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                orderId: 0,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes,
                SenderStateProvinceId: model.SenderStateProvinceId,
                ReciverCountryId: model.ReciverCountryId,
                ReciverStateProvinceId: model.ReciverStateProvinceId,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName);

            var profit = _orderReportService.ProfitReport(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            gridModel.ExtraData = new
            {
                orderAggreratorModel = new OrderAggreratorModel()
                {
                    aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                    aggregatorshipping = _priceFormatter.FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false),
                    aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
                    aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumOrders, true, false)
                },
                CoardinationStatistic = _CoardinationStatistic
            };

            return Json(gridModel);
        }
        public string SumProductName(ICollection<OrderItem> items)
        {
            return string.Join(",",
            items.Where(p => p.Product != null).ToList().Select(p => p.Product.Name).Distinct().ToList());
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult EditCoordinationDate(int OrderId, string CoordinationDate)
        {
            try
            {
                DateTime? _CoordinationDate = null;
                if (!string.IsNullOrEmpty(CoordinationDate))
                    _CoordinationDate = Convert.ToDateTime(CoordinationDate);
                _extendedShipmentService.EditPostCoordination(OrderId, _CoordinationDate);
                return Json(new { success = true, message = "به روز رسانی با موفقیت انجام شد" });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = true, message = "خطا در زمان به روز رسانی" });
            }
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        [HttpsRequirement(SslRequirement.Yes)]
        [AdminAntiForgery]
        [ValidateIpAddress]
        [AuthorizeAdmin]
        [ValidateVendor]
        public IActionResult getOrderTabStatistic()
        {
            var result = _extendedShipmentService.getOrderByStateCount();
            return Json(result);
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
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult chooseCollector(int[] shipmentIds, int Collectorid, string Desc)
        {
            int i = 0;
            foreach (var shipmentId in shipmentIds)
            {
                if (_collectorService.ChooseCollector(shipmentId, Collectorid, Desc))
                    i++;
            }
            if (i == 0)
                return Json(new { success = false, message = "انتصابی انجام نشد" });
            return Json(new { success = true, message = "انتصاب به جمع آور موفقیت انجام شد" });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult _getshipmentIds(int customerId, int stateId, int countryId, string Address, int shipmentState)
        {
            try
            {
                int TotalWeight = 0;
                int count = 0;
                var shipments = _extendedShipmentService.GetAllShipments(out count,
               billingCountryId: countryId,
               billingStateId: stateId,
               OrderCustomerId: customerId,
               pageIndex: 0,
               pageSize: 5000,
               ShipmentState: 0,
               ShipmentState2: shipmentState);
                List<Address> addresses = new List<Address>();
                if (!shipments.Any())
                {
                    return Json(new { success = false, message = "محموله ای جهت انتصاب به جمع آور یافت نشد" });
                }
                List<int> _newShipment = new List<int>();
                foreach (var shipment in shipments)
                {
                    var _shipment = _shipmentService.GetShipmentById(shipment.Id);
                    var realShipment = _shipmentService.GetShipmentById(shipment.Id);
                    int serviceId = realShipment.Order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    if (!string.IsNullOrEmpty(Address))
                    {
                        if ((_shipment.Order.Customer.Username + " - " + _shipment.Order.BillingAddress.Address1 + "-" + (_shipment.Order.BillingAddress.FirstName ?? "") + " " + (_shipment.Order.BillingAddress.LastName ?? "")).Trim() != Address.Trim())
                        {
                            continue;
                        }
                    }
                    if (!_extendedShipmentService.IsShipmentCollected(shipment.Id))
                        _newShipment.Add(shipment.Id);
                }
                if (!_newShipment.Any())
                    return Json(new { success = false, message = "محموله ای جهت انتصاب به جمع آور یافت نشد" });
                return Json(new { success = true, shipmentIds = string.Join(",", _newShipment.ToList()) });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult ShipmentListSelect(DataSourceRequest command, ExtendShipmentListModel model)
        {
            //if (_workContext.CurrentCustomer.IsInCustomerRole("nazer-post"))
            //{
            //    if (_workContext.CurrentCustomer.BillingAddress == null)
            //        return null;
            //    model.CountryId = _workContext.CurrentCustomer.BillingAddress.CountryId.GetValueOrDefault(65536000);
            //    if (model.CountryId == 1)
            //        model.StateProvinceId =
            //            _workContext.CurrentCustomer.BillingAddress.StateProvinceId.GetValueOrDefault(65536000);
            //}

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments

            int count = 0;
            var shipments = _extendedShipmentService.GetAllShipments(out count, vendorId: vendorId,
                warehouseId: model.WarehouseId,
                billingCountryId: model.treeCountryId > 0 ? model.treeCountryId : model.CountryId,
                billingStateId: model.treeStateId > 0 ? model.treeStateId : model.StateProvinceId,
                OrderCustomerId: model.treeCustomerId,
                billingCity: model.City,
                shippingCountryId: model.ReciverCountryId,
                shippingStateId: model.ReciverStateProvinceId,
                shippingCity: model.ReciverCity,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                orderId: model.orderId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                PostmanId: model.PostmanId,
                ShipmentState: model.ShipmentState,
                ShipmentState2: model.ShipmentState2);
            if (shipments.Any())
            {
                count = shipments.First().Count;
            }
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = count//shipments.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult ShipmentListSelectByStatus(DataSourceRequest command, ExtendShipmentListModel model)
        {
            //if (_workContext.CurrentCustomer.IsInCustomerRole("nazer-post"))
            //{
            //    if (_workContext.CurrentCustomer.BillingAddress == null)
            //        return null;
            //    model.CountryId = _workContext.CurrentCustomer.BillingAddress.CountryId.GetValueOrDefault(65536000);
            //    if (model.CountryId == 1)
            //        model.StateProvinceId =
            //            _workContext.CurrentCustomer.BillingAddress.StateProvinceId.GetValueOrDefault(65536000);
            //}

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;
            if (model.StatusId == -1)
            {
                model.StatusId = 0;
            }
            //load shipments
            int count = 0;
            var shipments = _extendedShipmentService.GetAllShipmentsByStatus(out count, vendorId: vendorId,
                warehouseId: model.WarehouseId,
                billingCountryId: model.treeCountryId > 0 ? model.treeCountryId : model.CountryId,
                billingStateId: model.treeStateId > 0 ? model.treeStateId : model.StateProvinceId,
                OrderCustomerId: model.treeCustomerId,
                billingCity: model.City,
                shippingCountryId: model.ReciverCountryId,
                shippingStateId: model.ReciverStateProvinceId,
                shippingCity: model.ReciverCity,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                orderId: model.orderId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                PostmanId: model.PostmanId,
                ShipmentState: model.ShipmentState,
                ShipmentState2: model.ShipmentState2,
                statusId: model.StatusId);
            if (shipments.Any())
            {
                count = shipments.First().Count;
            }
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = count//shipments.TotalCount
            };

            return Json(gridModel);
        }
        protected void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder", _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
        }
        [HttpGet]
        [Area(AreaNames.Admin)]
        public IActionResult getUserInRole(int? country, int? State, string City, CustomerType customerType)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);
            if (extendedShipmentSetting != null)
            {
                int roleId = (customerType == CustomerType.PostAdmin ?
                    extendedShipmentSetting.PostAdminRoleId :
                    extendedShipmentSetting.PostmanRoleId);
                var data = _extendedShipmentService.getUserInRole(roleId, country, State, City);
                return Json(data);
            }
            return Content(extendedShipmentSetting.PostmanRoleId.ToString());
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult SetPostMan(int shipmentId, int PostManId)
        {
            int PostAdminId = 0;
            _extendedShipmentService.ChossePostMan(PostManId, PostAdminId, shipmentId);
            //send sms
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Json(new { IsOk = true });
        }
        [Area(AreaNames.Admin)]
        public IActionResult GetBarcodeFromPost(int shipmentId)
        {
            //var shipment = _shipmentService.GetShipmentById(shipmentId);
            //int senderStateId = 0;
            //if (!_extendedShipmentService.getDefulteSenderState(shipment.Order.CustomerId, out senderStateId))
            //{
            //    senderStateId = shipment.Order.BillingAddress.StateProvinceId.Value;
            //}
            //var trackingNumber = _extendedShipmentService.GenerateBarcodeFromPost(shipmentId, senderStateId);
            //if (!string.IsNullOrEmpty(trackingNumber))
            //{

            //    shipment.TrackingNumber = trackingNumber;
            //    _shipmentService.UpdateShipment(shipment);

            //    _extendedShipmentService.SetPersuitCodeMode(shipmentId, true);
            //    return Json(new { IsOk = true, barcode = trackingNumber });
            //}
            return Json(new { IsOk = false });
        }
        [Area(AreaNames.Admin)]
        public IActionResult SendDateToPost(int shipmentId)
        {
            String result = _extendedShipmentService.SendDataToPost(shipmentId);
            if (string.IsNullOrEmpty(result))
            {
                return Json(new { IsOk = true });
            }
            return Json(new { IsOk = false, msg = result });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult getUpdateFromPost(int shipmentId)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);
            if (extendedShipmentSetting != null)
            {
                var shipment = _shipmentService.GetShipmentById(shipmentId);
                if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                {
                    _extendedShipmentService.UpdateFromPost(shipment);
                    return Json(new { IsOk = true, Message = "محموله مورد نظر فاقد بارکد می باشد" });
                }
                return Json(new { IsOk = false });
            }
            return Json(new { IsOk = false, Message = "ابتدا تنظیمات ارتباط با سرویس پست را وارد نمایید" });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult getUpdateFromPostByOrderID(int orderId)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);

            //var order = _orderService.GetOrderById(orderId);
            int count = 0;
            var ordersShipments = _extendedShipmentService.getOrderShipment(out count, orderId, 0, 1000);

            if (extendedShipmentSetting != null)
            {
                try
                {
                    foreach (var item in ordersShipments)
                    {
                        var shipment = _shipmentService.GetShipmentById(item.ShipmentId);
                        if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                        {
                            _extendedShipmentService.UpdateFromPost(shipment);
                        }
                    }
                    return Json(new { IsOk = true, Message = "عملیات با موفقیت انجام شد" });
                }
                catch
                {
                    return Json(new { IsOk = false, Message = "محموله مورد نظر فاقد بارکد می باشد" });
                }

            }
            else
            {
                return Json(new { IsOk = false, Message = "ابتدا تنظیمات ارتباط با سرویس پست را وارد نمایید" });
            }
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult getUpdateFromPostByOrderIDs(int[] ordersIds)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);
            //var order = _orderService.GetOrderById(orderId);
            foreach (var orderId in ordersIds)
            {
                int count = 0;
                var ordersShipments = _extendedShipmentService.getOrderShipment(out count, orderId, 0, 1000);

                if (extendedShipmentSetting != null)
                {
                    try
                    {
                        foreach (var item in ordersShipments)
                        {
                            var shipment = _shipmentService.GetShipmentById(item.ShipmentId);
                            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                            {
                                _extendedShipmentService.UpdateFromPost(shipment);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
                else
                {

                    return Json(new { IsOk = false, Message = "ابتدا تنظیمات ارتباط با سرویس پست را وارد نمایید" });
                }
            }
            return Json(new { IsOk = true, Message = "عملیات با موفقیت انجام شد" });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult CancelOrderByOrderIDs(int[] ordersIds)
        {
            int ErrorCount = 0;
            if (ordersIds.Count() > 0)
            {
                foreach (var orderId in ordersIds)
                {
                    try
                    {
                        var order = _orderService.GetOrderById(orderId);
                        if (order == null)
                            return Json(new { IsOk = false, message = "سفارش مورد نظر یافت نشد" });
                       
                        if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                            return Json(new { IsOk = false, message = "سفارش مورد نظر قبلا کنسل شده" });
                        _orderProcessingService.CancelOrder(order, true);
                        common.InsertOrderNote($"سفارش مورد توسط کاربر پشتیبانی {_workContext.CurrentCustomer.Username} به صورت انبوه کنسل شده", orderId);
                    }
                    catch (Exception ex)
                    {
                        ErrorCount++;
                        LogException(ex);
                    }
                }
            }
            if (ErrorCount == 0)
                return Json(new { IsOk = true, message = "عملیات با موفقیت انجام شد" });
            else
                return Json(new { IsOk = true, message = $"عملیات با موفقیت انجام شد ولی تعداد {ErrorCount} عدد از سفارشات با خطا مواجه شد" });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult setDataCollect(int shipmentId)
        {
            bool resultOf = _extendedShipmentService.setDataCollect(shipmentId);
            return Json(new { success = resultOf });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult setDataCollectAll(int[] shipmentIds)
        {
            foreach (var item in shipmentIds)
            {
                _extendedShipmentService.setDataCollect(item);
            }
            return Json(new { success = true });
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult setShipmentState(int[] shipmentIds, int ShipmentState)
        {
            try
            {
                string msg;
                var TrackingNumbers = _rewardPointCashoutService.execSp<string>("Shipment_Sp_GetShipmentDetail", new { shipmentIds = string.Join(",", shipmentIds), ShipmentState = ShipmentState, customerId = _workContext.CurrentCustomer.Id }, out msg);

                if (!string.IsNullOrEmpty(msg))
                {
                    return Json(new { success = false, message = msg });
                }
                int gatewayState = (ShipmentState == 1 ? 2 : 1);

                foreach (var item in TrackingNumbers)
                {
                    string res;
                    var bres = _codService.ShipmentChangeStatus(gatewayState, item, out res);
                    if (!bres) return Json(new { success = false, message = $"Error While Change State, Tracking Number = {item + Environment.NewLine + res}" });
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult getSmsItemsForPostMan(int[] shipmentIds)
        {
            try
            {
                int TotalWeight = 0;
                List<Address> addresses = new List<Address>();
                foreach (var item in shipmentIds)
                {
                    if (_notificationService.IsSendedSmsToPostman(item))
                    {
                        //return Json(new { success = false, message = "کاربر محترم شما برای این مرسوله قبلا به پستچی پیام ارسال کرده اید " });
                        continue;
                    }

                    var shipment = _shipmentService.GetShipmentById(item);
                    TotalWeight += _extendedShipmentService.GetItemWeightFromAttr(shipment.ShipmentItems.First().OrderItemId);
                    addresses.Add(shipment.Order.BillingAddress);
                }
                if (addresses.Count == 0)
                {
                    return Json(new { success = false, message = "کاربر محترم شما برای این مرسوله ها قبلا به پستچی پیام ارسال کرده اید " });
                }
                string SenderName = _workContext.CurrentCustomer.GetFullName();

                string msg = $@"جناب آقای........
باری با تعداد 1 بسته و وزن تقریبی {TotalWeight} در آدرس {
                    string.Join("\r\n", addresses.Select(p => p.SumAddress1()).Distinct().ToList())}
جهت جمع آوری به شما ارجاع گردید لطفا جمع آوری فرمائید
{SenderName} {_workContext.CurrentCustomer.Username}";
                return Json(new { success = true, weight = TotalWeight, message = msg, shipmentIds = string.Join(",", shipmentIds) });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, weight = 0 });
            }
        }
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult _getSmsItemsForPostMan(int customerId, int stateId, int countryId, string Address)
        {
            try
            {
                int TotalWeight = 0;
                int count = 0;
                List<CollectorSmsItem> Lst_Col = new List<CollectorSmsItem>();
                var shipments = _extendedShipmentService.GetAllShipments(out count,
                    billingCountryId: countryId,
                    billingStateId: stateId,
                    OrderCustomerId: customerId,
                    pageIndex: 0,
                    pageSize: 5000,
                    ShipmentState: 0,
                    ShipmentState2: 4);
                List<Address> addresses = new List<Address>();
                if (!shipments.Any())
                {
                    return Json(new { success = false, message = "مرسوله ای جهت ارسال پیامک یافت نشد" });
                }
                int i = 0;
                foreach (var shipment in shipments)
                {
                    //if (_notificationService.IsSendedSmsToPostman(shipment.Id))
                    //{
                    //    //return Json(new { success = false, message = "کاربر محترم شما برای این مرسوله قبلا به پستچی پیام ارسال کرده اید " });
                    //    continue;
                    //}
                    var _shipment = _shipmentService.GetShipmentById(shipment.Id);
                    if (!string.IsNullOrEmpty(Address))
                    {
                        if ((_shipment.Order.Customer.Username + " - " + _shipment.Order.BillingAddress.Address1 + "-" + (_shipment.Order.BillingAddress.FirstName ?? "") + " " + (_shipment.Order.BillingAddress.LastName ?? "")).Trim() != Address.Trim())
                        {
                            continue;
                        }
                    }
                    int OrderItemId = _shipment.ShipmentItems.First().OrderItemId;
                    int weight = _extendedShipmentService.getItemWeight_V(OrderItemId);
                    var dimantions = _extendedShipmentService.getDimensions(OrderItemId);
                    Lst_Col.Add(new CollectorSmsItem() { Address = _shipment.Order.BillingAddress, Weight = weight, ShipmentId = shipment.Id });
                    i++;
                }
                if (Lst_Col.Count == 0)
                {
                    return Json(new { success = false, message = "کاربر محترم شما برای این مرسوله ها قبلا به پستچی پیام ارسال کرده اید " });
                }

                var data = Lst_Col.GroupBy(p => p.Address.SumAddress1()).Select(n => new
                {
                    _Address = n.First().Address.SumAddress1(),
                    Weight = n.Sum(p => p.Weight),
                    _Count = n.Count(),
                    shipmentIds = string.Join(",", n.Select(p => p.ShipmentId.ToString()).ToList())
                }).ToList();
                string SenderName = _workContext.CurrentCustomer.GetFullName();

                List<CollectorSmsItem> _SmsModel = new List<CollectorSmsItem>();
                foreach (var item in data)
                {
                    string _SenderName = _workContext.CurrentCustomer.IsInCustomerRole("Administrators") ? "" : $"{SenderName} {_workContext.CurrentCustomer.Username}";
                    string Msg = ($@"تعداد {item._Count} بسته و وزن تقریبی {item.Weight} :گرم در آدرس { item._Address} لطفا جهت جمع آوری اقدام فرمائید " + _SenderName).Trim();
                    _SmsModel.Add(new CollectorSmsItem()
                    {
                        _Address = item._Address,
                        Weight = item.Weight,
                        _Count = item._Count,
                        Sms = Msg,
                        shipmentIds = item.shipmentIds
                    });
                }
                string Str_SmsModel = Newtonsoft.Json.JsonConvert.SerializeObject(_SmsModel);
                return Json(new { success = true, SmsModel = Str_SmsModel });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, weight = 0 });
            }
        }
        public class CollectorSmsItem
        {
            public Address Address { get; set; }
            public string _Address { get; set; }
            public int Weight { get; set; }
            public int _Count { get; set; }
            public string Sms { get; set; }
            public int ShipmentId { get; set; }
            public string shipmentIds { get; set; }
            public string Dimantions { get; set; }

        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult SendSmsToPostMan(string mobile, string Str_SmsModel)
        {
            try
            {
                if (string.IsNullOrEmpty(Str_SmsModel))
                    return Json(new { success = false, message = "متن پیام نامعتبر می باشد" });
                if (string.IsNullOrEmpty(mobile) || mobile.Length != 11)
                {
                    return Json(new { success = false, message = "شماره موبایل وارد شده نامعتبر می باشد" });
                }
                var smsItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CollectorSmsItem>>(Str_SmsModel);

                foreach (var item in smsItems)
                {
                    if (string.IsNullOrEmpty(item.Sms))
                        continue;
                    item.Sms = item.Sms.Trim();
                    if (item.Sms.Length > 268)
                        item.Sms = item.Sms.Substring(0, 267);
                    bool sms = _notificationService._sendSms(mobile, item.Sms);
                    foreach (var _item in item.shipmentIds.Split(','))
                    {
                        var shipment = _shipmentService.GetShipmentById(int.Parse(_item));
                        _notificationService.InsertNotifyPostMan(int.Parse(_item), item.Sms, mobile);
                    }
                }

                return Json(new { success = true, message = "پیام با موفقیت ارسال شد" });

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "خطا در زمان ارسال پیام" });
            }
        }
        [Area(AreaNames.Admin)]
        public IActionResult GetPdf58mInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var order = _orderService.GetOrderById(orderId);
            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _extendedShipmentService.Print_58mToPdf(orders, stream);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ImageJpeg, $"order_{order.Id}.jpg");
        }

        public IActionResult PrintLable50MM(int orderId)
        {


            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var order = _orderService.GetOrderById(orderId);
            if (_workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _extendedShipmentService.PrintLable50MM(order, stream);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }
        #endregion

        #region Customer Area

        [HttpsRequirement(SslRequirement.Yes)]
        [HttpPost]
        public IActionResult GetPostCode(int StateId)
        {
            if (StateId == 0)
                return Json(new { zipPostCodeMask = "" });
            var CityCode = _repositoryStateCode.Table.FirstOrDefault(p =>
            p.stateId == StateId);
            if (CityCode == null)
                return Json(new { zipPostCodeMask = "" });
            if (string.IsNullOrEmpty(CityCode.StateCode))
                return Json(new { zipPostCodeMask = "" });
            int[] tehranManategh = new int[] { 585, 4, 579, 580, 582, 583, 584, 581 };
            string StateCode = CityCode.StateCode;
            if (tehranManategh.Contains(StateId))
            {
                StateCode = StateCode.Replace("10", "");
            }
            int count = 10 - StateCode.Length;
            return Json(new
            {
                zipPostCodeMask = StateCode + new string('1', count),
                cityCodeLenght = StateCode.Length
            });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        [HttpPost]
        public IActionResult GetAddressLatLong(int AddressId)
        {
            if (AddressId == 0)
                return Json(new { lat = 0, lon = 0 });
            var result = _extendedShipmentService.GetAddressLocation(AddressId);
            if (result == null)
                return Json(new { lat = 0, lon = 0 });
            return Json(new { lat = result.Lat, lon = result.Long });
        }
        [HttpsRequirement(SslRequirement.Yes)]
        public override IActionResult CustomerOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            var model1 = _orderModelFactory.PrepareCustomerOrderListModel();
            var model = new OrderListModel
            {
                AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList()
            };
            model.AvailableOrderStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            //if (orderStatusIds != null && orderStatusIds.Any())
            //{
            //    foreach (var item in model.AvailableOrderStatuses.Where(os => orderStatusIds.Contains(os.Value)))
            //        item.Selected = true;
            //    model.AvailableOrderStatuses.First().Selected = false;
            //}
            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            //if (paymentStatusIds != null && paymentStatusIds.Any())
            //{
            //    foreach (var item in model.AvailablePaymentStatuses.Where(ps => paymentStatusIds.Contains(ps.Value)))
            //        item.Selected = true;
            //    model.AvailablePaymentStatuses.First().Selected = false;
            //}
            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            //if (shippingStatusIds != null && shippingStatusIds.Any())
            //{
            //    foreach (var item in model.AvailableShippingStatuses.Where(ss => shippingStatusIds.Contains(ss.Value)))
            //        item.Selected = true;
            //    model.AvailableShippingStatuses.First().Selected = false;
            //}

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            //vendors
            model.AvailableVendors = null;

            //warehouses
            model.AvailableWarehouses = null;

            //payment methods
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var pm in _paymentService.LoadAllPaymentMethods())
                model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
            {
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //a vendor should have access only to orders with his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderListModel, ExtendedOrderListModel>();
            });
            IMapper mapper = config.CreateMapper();
            ExtendedOrderListModel eolModel = (ExtendedOrderListModel)mapper.Map(model, typeof(OrderListModel), typeof(ExtendedOrderListModel));
            CustomerOrderMixModel Parentmodel = new CustomerOrderMixModel()
            {
                ColModel = model1,
                EolModel = eolModel
            };
            return View(Parentmodel);
        }
        [HttpsRequirement(SslRequirement.Yes)]
        public IActionResult NewCustomerOrders(ExtendedOrderListModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var PreparedModel = _extnOrderModelFactory.ExnPrepareCustomerOrderListModel(
                customerId: _workContext.CurrentCustomer.Id,
                OrderId: model.OrderId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                OrderStatusIds: orderStatusIds,
                paymentStatusIds: paymentStatusIds,
                shippingStatusIds: shippingStatusIds,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes,
                pageIndex: 0,
                pageSize: int.MaxValue,
                SenderStateProvinceId: model.SenderStateProvinceId,
                ReciverCountryId: model.ReciverCountryId,
                ReciverStateProvinceId: model.ReciverStateProvinceId,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName);
            return Json(PreparedModel);
        }

        protected ExtendedShipmentModel PrepareShipmentModel(ExtendedShipmentModel shipment, bool prepareProducts
            , bool prepareShipmentEvent = false)
        {
            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var model = new ExtendedShipmentModel
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                strTotalWeight = shipment.TotalWeight.HasValue ? $"{shipment.TotalWeight:F2} [{baseWeightIn}]" : "",
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                //CustomOrderNumber = shipment.Order.CustomOrderNumber,
                postManName = shipment.postManName,
                orderPrice = shipment.orderPrice,
                HasDiffrentWeight = shipment.HasDiffrentWeight,
                DataCollect = shipment.DataCollect,
                FullAddress = (shipment.BillingCountryName ?? "") + "-" + (shipment.BillingStateProvinceName ?? "") + "-" + (shipment.Address1 ?? ""),
                HasBime = shipment.HasBime,
                LastState = shipment.LastState,
                IsBulk = shipment.IsBulk,
                Count = shipment.Count,
                delayDataCollect = shipment.delayDataCollect,
                delayShippedDate = shipment.delayShippedDate,
                CoordinationDate = shipment.CoordinationDate,
                NeedCarton = shipment.NeedCarton,
                NeedPrinter = shipment.NeedPrinter,
                CartonSizeName = shipment.CartonSizeName,
                PostManMobile = shipment.PostManMobile,
                IdCategory = shipment.IdCategory,
                IsForeign = shipment.IsForeign,
                LatestEventId = shipment.LatestEventId

            };

            if (prepareProducts)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    //quantities
                    var qtyInThisShipment = shipmentItem.Quantity;
                    var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = orderItem.Quantity;
                    var qtyInAllShipments = orderItem.GetTotalNumberOfItemsInAllShipment();

                    var warehouse = _shippingService.GetWarehouseById(shipmentItem.WarehouseId);
                    var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                    {
                        Id = shipmentItem.Id,
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.Product.Name,
                        Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                        AttributeInfo = orderItem.AttributeDescription,
                        ShippedFromWarehouse = warehouse != null ? warehouse.Name : null,
                        ShipSeparately = orderItem.Product.ShipSeparately,
                        ItemWeight = orderItem.ItemWeight.HasValue ? $"{orderItem.ItemWeight:F2} [{baseWeightIn}]" : "",
                        ItemDimensions = $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimensionIn}]",
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                    };
                    //rental info
                    if (orderItem.Product.IsRental)
                    {
                        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                        shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);
                    }

                    model.Items.Add(shipmentItemModel);
                }
            }

            if (!prepareShipmentEvent || string.IsNullOrEmpty(shipment.TrackingNumber))
                return model;

            var shipmentTracker = shipment.GetShipmentTracker(_shippingService, _shippingSettings);
            if (shipmentTracker == null)
                return model;

            model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
            if (!_shippingSettings.DisplayShipmentEventsToStoreOwner)
                return model;

            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
            if (shipmentEvents == null)
                return model;

            foreach (var shipmentEvent in shipmentEvents)
            {
                var shipmentStatusEventModel = new ShipmentModel.ShipmentStatusEventModel();
                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                shipmentStatusEventModel.Country = shipmentEventCountry != null
                    ? shipmentEventCountry.GetLocalized(x => x.Name)
                    : shipmentEvent.CountryCode;
                shipmentStatusEventModel.Date = shipmentEvent.Date;
                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                shipmentStatusEventModel.Location = shipmentEvent.Location;
                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
            }

            return model;
        }
        #endregion

        [HttpGet]
        [Area(AreaNames.Admin)]
        public IActionResult ChangeOrderStatus(int shipmentId, int statusId, string description)
        {
            _shipmentTrackingService.InsertTracking(shipmentId, (OrderShipmentStatusEnum)statusId, description);
            return Ok();
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult ChangeOrderStatus([FromQuery] int statusId, [FromQuery] string description, [FromBody] ExtendShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;
            if (model.StatusId == -1)
            {
                model.StatusId = 0;
            }

            //load shipments
            int count = 0;
            var shipments = _extendedShipmentService.GetAllShipmentsByStatus(out count, vendorId: vendorId,
                warehouseId: model.WarehouseId,
                billingCountryId: model.treeCountryId > 0 ? model.treeCountryId : model.CountryId,
                billingStateId: model.treeStateId > 0 ? model.treeStateId : model.StateProvinceId,
                OrderCustomerId: model.treeCustomerId,
                billingCity: model.City,
                shippingCountryId: model.ReciverCountryId,
                shippingStateId: model.ReciverStateProvinceId,
                shippingCity: model.ReciverCity,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                orderId: model.orderId,
                pageIndex: 0,
                pageSize: 100,
                PostmanId: model.PostmanId,
                ShipmentState: model.ShipmentState,
                ShipmentState2: model.ShipmentState2,
                statusId: model.StatusId);

            foreach (var item in shipments)
            {
                _shipmentTrackingService.InsertTracking(item.Id, (OrderShipmentStatusEnum)statusId, description);
            }
            return Ok();
        }


        //[AdminAntiForgery]
        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult TrackForeign(int _OrderId, int _Status, string _Des, int _Mablagh, int _ShipingId)
        {
            bool State_insert = false;
            #region Get Current Status Order
            Order Current_Order = _orderService.GetOrderById(_OrderId);
            #endregion
            if (Current_Order.OrderStatusId == (int)OrderStatus.Cancelled)
            {
                return Json(new { success = true, message = "کاربر گرامی ، سفارش کنسل شده است، امکان تکمیل شدن وجود ندارد" });
            }
            else
            {

                #region select from tbl Related orde
                Tb_RelatedOrders tb_RelatedOrders = _tb_RelatedOrders_Service.GetTb_RelatedOrders_By_ParentOrderId(_OrderId);
                Order OrderRelated = _orderService.GetOrderById(tb_RelatedOrders.ChildOrderId);
                #endregion

                #region select customer
                Customer customer = _customerService.GetCustomerById(_OrderId);

                #endregion
                #region select shiping
                var current_shiping = _shipmentService.GetShipmentById(_ShipingId);
                #endregion

                #region اگر تایید بود باید سفارش پیشتاز این سفارش تکمیل بشه
                switch (_Status)
                {
                    case 1://عدم تایید سفارش
                        if (current_shiping.ShippedDateUtc == null)//OrderStatus.Pending.CompareTo(OrderRelated.OrderStatusId) == 0 &&
                        {
                            CancelOrder(_OrderId);
                            CancelOrder(OrderRelated.Id);
                            State_insert = true;
                        }
                        else
                        {
                            State_insert = true;
                            return Json(new { success = true, message = "کاربر گرامی ، سفارش پیشتاز داخلی این سفارش ارسال شده است، امکان کنسل شدن وجود ندارد" });
                        }
                        break;
                    case 2://تایید
                        _extendedShipmentService.InsertOrderNote("سفارش پست خارجی از سوی شرکت مربوطه تایید شد و منتظر پرداخت مشتری می باشد", Current_Order.Id);
                        bool sms1 = _notificationService._sendSms(Current_Order.BillingAddress.PhoneNumber, $@" مشتری محترم سفارش پست خارجی شما تایید شد. لطفا جهت شروع فرآیند جمع آوری و ارسال نسبت به پرداخت مبلغ سفارش در قسمت 'مرسولات من' در سامانه پستِکس اقدام فرمایید.
شماره سفارش:{Current_Order.Id.ToString()}");
                        break;

                    case 3://اعلام مفایرت افزایش
                        {
                            int AddedEngValue = 0;
                            AddedEngValue = (_Mablagh * 5) / 100;
                            int finalAddedValue = (_Mablagh + AddedEngValue) + (((_Mablagh + AddedEngValue) * 9) / 100);
                            Current_Order.OrderTotal += finalAddedValue;
                            _orderService.UpdateOrder(Current_Order);
                            _extendedShipmentService.InsertOrderNote("مبلغ سفارش افزایش یافت، اضافه شده:" + finalAddedValue.ToString("N0"), Current_Order.Id);
                            bool sms2 = _notificationService._sendSms(Current_Order.BillingAddress.PhoneNumber, $@"مشتری محترم مبلغ سفارش پست خارجی به مبلغ {finalAddedValue.ToString("N0")} ريال افزایش داشته و مبلغ نهایی سفارش شما {Convert.ToInt32(Current_Order.OrderTotal).ToString("N0")} ريال می باشد
.لطفا جهت شروع فرآیند جمع آوری و ارسال نسبت به پرداخت مبلغ سفارش در قسمت 'مرسولات من' در سامانه پستِکس اقدام فرمایید.
شماره سفارش:{Current_Order.Id.ToString()}
");
                            break;
                        }
                    case 4:
                        int AddedEngValue1 = 0;
                        AddedEngValue1 = (_Mablagh * 5) / 100;
                        int finalAddedValue1 = (_Mablagh + AddedEngValue1) + (((_Mablagh + AddedEngValue1) * 9) / 100);
                        Current_Order.OrderTotal -= finalAddedValue1;
                        _orderService.UpdateOrder(Current_Order);
                        _extendedShipmentService.InsertOrderNote("مبلغ سفارش کاهش یافت، مبلغ کم شده:" + finalAddedValue1.ToString("N0"), Current_Order.Id);
                        bool sms3 = _notificationService._sendSms(Current_Order.BillingAddress.PhoneNumber, $@"مشتری محترم مبلغ سفارش پست خارجی به مبلغ {finalAddedValue1.ToString("N0")} ريال کاهش داشته و مبلغ نهایی سفارش شما {Convert.ToInt32(Current_Order.OrderTotal).ToString("N0")} ريال می باشد
.لطفا جهت شروع فرآیند جمع آوری و ارسال نسبت به پرداخت مبلغ سفارش در قسمت 'مرسولات من' در سامانه پستِکس اقدام فرمایید.
شماره سفارش:{Current_Order.Id.ToString()}
");
                        break;
                    default:
                        break;
                }
                #endregion

                #region insert in to table track
                bool resultinsert = _declaration_Status_Foreign_Order_Service.Insert_to_TrackingForeign(_OrderId, _Status, _Des, _Mablagh, _ShipingId);
                #endregion
                return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
                //if (State_insert)
                //{

                //}
                //else
                //{
                //    return Json(new { success = true, message = "!!!!!" });

                //}

            }

        }


        //[Area(AreaNames.Admin)]
        //[AdminAntiForgery]
        [HttpPost]
        [Area(AreaNames.Admin)]
        //[HttpPost]
        public IActionResult HistoryListTrackForeign(int _OrderId, int _ShipingId)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            List<Tbl_TrackingForeignOrder> Tbl_History = new List<Tbl_TrackingForeignOrder>();
            var gridModel = new DataSourceResult();
            var Final_history = new List<vm_GridHistory_TrackingForeign>();
            try
            {
                Tbl_History = _declaration_Status_Foreign_Order_Service.GetTbl_TrackingForeignOrders(_OrderId, _ShipingId);
                if (Tbl_History.Count > 0)
                {
                    Final_history = Tbl_History.Select(q => new vm_GridHistory_TrackingForeign()
                    {

                        Status = q.Status == 1 ? "عدم تایید" : q.Status == 2 ? " تایید" : q.Status == 3 ? "مغایرت-افزایش مبلغ" : q.Status == 4 ? "مغایرت-کاهش مبلغ" : "-",
                        Des = q.Description,
                        User = _customerService.GetCustomerById((int)q.IdUserInsert).GetFullName(),
                        Date = q.DateInsert,
                        Mablagh = q.Mablagh.ToString(),
                        IsPay = q.IsPay == true ? "پرداخت شده" : "عدم پرداخت"
                    }).ToList(); ;

                }
                else
                {
                    ErrrorMassege = "اطلاعاتی یافت نشد";
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                ErrrorMassege = "خطا در زمان واکشی اطلاعات";
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_history,
                    Total = Final_history.Count,
                    Errors = ErrrorMassege,
                };
            }
            return Json(gridModel);
        }

        public bool CancelOrder(int id)
        {
            if ((_workContext.CurrentCustomer == null || _workContext.CurrentCustomer.IsGuest() || _workContext.CurrentCustomer.Username == null))
                return false;

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return false;
            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;
            try
            {
                _orderProcessingService.CancelOrder(order, true);
                LogEditOrder(order.Id);
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    return true;
                }
                return false;
            }
            catch (Exception exc)
            {
                //error
                return false;
            }
        }




        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult ListOrderItem(int _OrderId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            var Grid = new List<vm_Grid_ListOrderItem_Ubaar>();//new DataSourceResult();//
            try
            {
                Order _order = _orderService.GetOrderById(_OrderId);
                var _OrderItems = _order.OrderItems.Where(p => p.ProductId == 10412).ToList();

                if (_OrderItems != null)
                {
                    Grid = _OrderItems.Select(q => new vm_Grid_ListOrderItem_Ubaar()
                    {
                        id = q.Id,
                        name = _productService.GetProductById(q.ProductId).Name + q.AttributeDescription,
                        price = _calcPriceOrderItem_Service.Get_IncomePrice_by_OrderItemId(q.Id)
                    }
                        ).ToList();
                }

            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return Json(Grid);
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult TrackUbaar(int _OrderId, int _Status, string _Des, senditem List_PP)
        {

            #region Get Current Status Order
            Order Current_Order = _orderService.GetOrderById(_OrderId);
            #endregion
            if (Current_Order.OrderStatusId == (int)OrderStatus.Cancelled)
            {
                return Json(new { success = true, message = "کاربر گرامی ، سفارش کنسل شده است، امکان تکمیل شدن وجود ندارد" });
            }
            else
            {



                #region select customer
                Customer customer = _customerService.GetCustomerById(_OrderId);

                #endregion


                #region 
                switch (_Status)
                {
                    case 1://بروز رسانی قیمت
                        if (List_PP.Listitem.Count() > 0)
                        {
                            foreach (var item1 in List_PP.Listitem)
                            {

                                _calcPriceOrderItem_Service.Update_IncomePrice_by_OrderItemId(item1._OrderItemId, item1._NewPrice);
                                Current_Order.OrderTotal = Current_Order.OrderTotal + (item1._NewPrice - item1._Price);
                                _orderService.UpdateOrder(Current_Order);
                                _trackingUbaarOrder_Service.Insert(_OrderId, item1._OrderItemId, _Status, _Des, item1._Price, item1._NewPrice);
                                string Link = "http://postex.ir/Dashboard/CustomerConfirmOrderUbaar/" + item1._OrderItemId.ToString(); ;
                                bool sms = _notificationService._sendSms(customer.Username, "کاربرگرامی مبلغ سفارش شما به شماره: " + _OrderId.ToString() + "توسط راننده مبلغ: " + Current_Order.OrderTotal.ToString() + "ریال اعلام شده است، جهت تایید سفارش برروی لینک زیر کلیک نمایید: " + Link);
                            }

                            #region log sms
                            //Tbl_LogSMS log1 = new Tbl_LogSMS();
                            //log1.Type = 6;
                            //log1.Mobile = _workContext.CurrentCustomer.Username;
                            //log1.StoreId = _storeContext.CurrentStore.Id;
                            //log1.TextMessage = msg;
                            //log1.Status = sended == true ? 1 : 0;
                            //log1.DateSend = DateTime.Now;
                            //_repositoryTbl_LogSMS.Insert(log1);

                            #endregion

                        }
                        else
                        {

                            return Json(new { success = true, message = "کاربر گرامی ،به دلیل عدم وجود جزییات سفارش امکان ویرایش اطلاعات وجود ندارد" });
                        }
                        break;
                    case 3://تایید راننده

                        _trackingUbaarOrder_Service.Insert(_OrderId, 0, _Status, _Des, 0, 0);
                        bool sms1 = _notificationService._sendSms(customer.Username, "کاربرگرامی برای سفارش شما به شماره: " + _OrderId.ToString() + " راننده با مبلغ: " + Current_Order.OrderTotal.ToString() + "ریال قبول کرده است، لطفا جهت پرداخت سفارش به حساب کاری مراجعه فرمایید: " + "http://postex.ir/Dashboard/Orders");
                        break;

                    default:
                        break;
                }
                #endregion

                return Json(new { success = true, message = "عملیات با موفقیت انجام شد" });
            }

        }


        [Area(AreaNames.Admin)]
        public IActionResult GetBikers()
        {
            var bikers = _collectorService.GetBikers();
            return Ok(bikers);
        }

        [Area(AreaNames.Admin)]
        public IActionResult GetPostServices()
        {
            return Ok(_phoneOrderService.ListOfService());
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult EventCategory()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/EventCategory/Index.cshtml");
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult GetAllCategories()
        {
            var categories = _repository_Category.TableNoTracking.Where(p => p.Published && p.Id != 671 && p.Id != 720 && p.ParentCategoryId != 0)
                .OrderBy(p => p.DisplayOrder).Select(p => new { p.Name, p.Id }).ToList();
            return Ok(categories);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult GetCategoriesByStatus([FromQuery] string statusId)
        {
            var eventCategories = _repository_ShipmentEventCategory.TableNoTracking.Where(p => p.ShipmentEventId == statusId)
                .Select(p => new { p.CategoryId });
            return Ok(eventCategories);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        public IActionResult UpdateCategoryStatus(CategoryStatus categoryStatus)
        {
            var statusString = categoryStatus.StatusId.ToString();
            var eventCategories = _repository_ShipmentEventCategory.Table.Where(p => p.ShipmentEventId == statusString).ToList();

            _repository_ShipmentEventCategory.Delete(
                eventCategories.Where(p => !categoryStatus.CategoryIds.Contains(p.CategoryId)).ToList());

            if (categoryStatus.CategoryIds != null)
            {
                _repository_ShipmentEventCategory.Insert(categoryStatus.CategoryIds
                    .Where(p => !eventCategories.Any(q => q.CategoryId == p))
                    .Select(p => new Tbl_ShipmentEventCategory()
                    {
                        ShipmentEventId = statusString,
                        CategoryId = p
                    }));
            }

            return Ok();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult GetOrderStatusByCategoryId([FromQuery] int categoryId)
        {
            var shipmentevents = _repository_ShipmentEventCategory.TableNoTracking.Where(p => p.CategoryId == categoryId).Select(p => p.ShipmentEventId).ToList();
            return Ok(_repository_ShipmentEvent.TableNoTracking.Where(p => shipmentevents.Contains(p.ShipmentEventId)).Select(p => new { Value = p.ShipmentEventId, Text = p.ShipmentEventName }).ToList());
        }

        [HttpPost]
        [Area(AreaNames.Admin)]
        public IActionResult IsAllowedToChangeStatus(ExtendShipmentListModel model)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;
            if (model.StatusId == -1)
            {
                model.StatusId = 0;
            }
            //load shipments
            int count = 0;
            var shipments = _extendedShipmentService.GetAllShipmentsByStatus(out count, vendorId: vendorId,
                warehouseId: model.WarehouseId,
                billingCountryId: model.treeCountryId > 0 ? model.treeCountryId : model.CountryId,
                billingStateId: model.treeStateId > 0 ? model.treeStateId : model.StateProvinceId,
                OrderCustomerId: model.treeCustomerId,
                billingCity: model.City,
                shippingCountryId: model.ReciverCountryId,
                shippingStateId: model.ReciverStateProvinceId,
                shippingCity: model.ReciverCity,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                orderId: model.orderId,
                pageIndex: 0,
                pageSize: 100,
                PostmanId: model.PostmanId,
                ShipmentState: model.ShipmentState,
                ShipmentState2: model.ShipmentState2,
                statusId: model.StatusId);

            var categoryIds = shipments.Select(p => p.IdCategory);
            if (categoryIds.Distinct().Count() != 1)
            {
                return Ok(false);
            }

            return GetOrderStatusByCategoryId(categoryIds.First());

        }


    }
}

