using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Kendoui;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{

    public class CodShipmentListController : BaseAdminController
    {
        #region Fields

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
        private readonly ICodService _codService;
        #endregion

        public CodShipmentListController(
            ICodService codService,
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
            ShippingSettings shippingSettings,
            ICountryService countryService
        )
        {
            _codService = codService;
            _countryService = countryService;
            _productAttributeParser = productAttributeParser;
            _measureService = measureService;
            _orderService = orderService;
            _extendedShipmentService = extendedShipmentService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
            _permissionService = permissionService;
            _measureSettings = measureSettings;
            _shippingService = shippingService;
            _localizationService = localizationService;
            _shippingSettings = shippingSettings;
        }

        public IActionResult Index()
        {
            if(!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
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

            return View("/Plugins/Orders.ExtendedShipment/Views/CodShipmentList.cshtml", model);
        }
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

            var startDateValue = model.StartDate == null
                ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null
                ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone)
                    .AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var count = 0;
            var shipments = _extendedShipmentService.GetAllShipments(out count, vendorId,
                model.WarehouseId,
                model.CountryId,
                model.StateProvinceId,
                model.City,
                model.ReciverCountryId,
                model.ReciverStateProvinceId,
                model.ReciverCity,
                model.TrackingNumber,
                model.LoadNotShipped,
                startDateValue,
                endDateValue,
                ReciverName: model.ReciverName,
                SenderName: model.SenderName,
                orderId: model.orderId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                PostmanId: model.PostmanId,
                ShipmentState: model.ShipmentState,
                ignorCod:2,
                CodEndTime:model.CodEndTime,
                CODShipmentEventId:model.CODShipmentEventId,
                CodTrackingDayCoun: model.CodTrackingDayCount,
                HasGoodsPrice: model.HasGoodsPrice);
            if (shipments.Any())
            {
                count = shipments.First().Count;
            }
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = count //shipments.TotalCount
            };

            return Json(gridModel);
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
                ShippedDate = shipment.ShippedDateUtc.HasValue
                    ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString()
                    : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue
                    ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString()
                    : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                //CustomOrderNumber = shipment.Order.CustomOrderNumber,
                postManName = shipment.postManName,
                orderPrice = shipment.orderPrice,
                HasDiffrentWeight = shipment.HasDiffrentWeight,
                DataCollect = shipment.DataCollect,
                FullAddress = (shipment.BillingCountryName ?? "") + "-" + (shipment.BillingStateProvinceName ?? "") +
                              "-" + (shipment.Address1 ?? ""),
                HasBime = shipment.HasBime,
                LastState = shipment.LastState,
                IsBulk = shipment.IsBulk,
                vaizShode = shipment.vaizShode,
                GoodsPrice = shipment.GoodsPrice,
                CanChargWallet = shipment.CanChargWallet,
                showWalletbnt = (shipment.CanChargWallet ==1 && shipment.GoodsPrice > 0),
                Count = shipment.Count
            };

            if (prepareProducts)
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
                        ItemDimensions =
                            $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimensionIn}]",
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd
                    };
                    //rental info
                    if (orderItem.Product.IsRental)
                    {
                        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                            ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value)
                            : "";
                        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                            ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value)
                            : "";
                        shipmentItemModel.RentalInfo = string.Format(
                            _localizationService.GetResource("Order.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);
                    }

                    model.Items.Add(shipmentItemModel);
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
        [HttpPost]
        public IActionResult PayForCod(int shipmentId,int price )
        {
            string masseage = "";
            var result = _codService.ChargeMoneyBagForCodGood(shipmentId, price,out masseage);
            return Json(new { masseage, result});
        }

    }
}