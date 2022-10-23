using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Events
{
    /// <summary>
    /// برگشت پول کارتون و لفاف قبلی در صورت ثبت درخواست بسته بندی
    /// </summary>
    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IPackingRequestService _packingRequestService;
        private readonly IOrderItemsRecordService _orderItemsRecordService;
        private readonly IAccountingService _accountingService;
        private readonly IShipmentService _shipmentService;
        private readonly IWebHelper _webHelper;
        private readonly IChargeWalletFailService _chargeWalletFailService;

        public OrderPaidEventConsumer(IProductAttributeParser productAttributeParser,
            IPackingRequestService packingRequestService,
            IOrderItemsRecordService orderItemsRecordService,
            IAccountingService accountingService,
            IShipmentService shipmentService,
            IWebHelper webHelper,
            IChargeWalletFailService chargeWalletFailService)
        {
            _productAttributeParser = productAttributeParser;
            _packingRequestService = packingRequestService;
            _orderItemsRecordService = orderItemsRecordService;
            _accountingService = accountingService;
            _shipmentService = shipmentService;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            try
            {
                if (!eventMessage.Order.OrderItems.Any())
                    return;
                var kartonList = eventMessage.Order.OrderItems.Where(p => p.ProductId == 10430).ToList();
                foreach (var item in kartonList)
                {
                    var attrs = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                    var shipmentIdAttr = attrs.FirstOrDefault(p => p.ProductAttribute.Name == "شماره محموله");
                    var shipmentId = _productAttributeParser.ParseValues(item.AttributesXml, shipmentIdAttr.Id).FirstOrDefault();
                    var shipmentIdInt = Convert.ToInt32(shipmentId);
                    var packingRequests = _packingRequestService.SearchPackingRequests(shipmentIdInt);
                    foreach (var pr in packingRequests.OrderByDescending(p => p.Id).GroupBy(p => p.ShipmentId).Select(p => p.FirstOrDefault()))
                    {
                        pr.Status = Enums.ShipmentPackingRequestStatus.Purchased;
                        _packingRequestService.UpdatePackingRequest(pr);
                        var shipment = _shipmentService.GetShipmentById(shipmentIdInt);
                        if (shipment != null)
                        {
                            var orderItemId = shipment.ShipmentItems.FirstOrDefault()?.OrderItemId;
                            if (orderItemId.HasValue)
                            {
                                var previousPackingPrice = _orderItemsRecordService.ShipmentHasPacking(orderItemId.Value);
                                if (previousPackingPrice > 0)
                                {
                                    //int rewardPointHistoryId = _accountingService.ChargeWalletForAgentSaleAmount(eventMessage.Order,
                                    //    previousPackingPrice,
                                    //    $"بازگشت مبلغ کارتن و لفاف محموله شماره {shipmentId} از شماره سفارش {eventMessage.Order.Id} به جهت اصلاح سایز کارتن",
                                    //    out string msg1);
                                    //if (rewardPointHistoryId > 0)
                                    //{
                                        _accountingService.InsertChargeWallethistory(new plugin.Orders.ExtendedShipment.Models.ChargeWalletHistoryModel()
                                        {
                                            //rewaridPointHistoryId = rewardPointHistoryId,
                                            orderId = eventMessage.Order.Id,
                                            CustomerId = eventMessage.Order.Customer.Id,
                                            orderItemId = item.Id,
                                            shipmentId = shipmentIdInt,
                                            ChargeWalletTypeId = 8,
                                            Description = $"بازگشت مبلغ کارتن و لفاف محموله شماره {shipmentId} از شماره سفارش {eventMessage.Order.Id} به جهت اصلاح سایز کارتن",
                                            Point = previousPackingPrice,
                                            IpAddress = _webHelper.GetCurrentIpAddress(),
                                            CreateDate = DateTime.Now
                                        });
                                    //}
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = eventMessage.Order.Id }, "OrderPaidEventConsumer  ->  برگشت پول کارتون و لفاف قبلی در صورت ثبت درخواست بسته بندی ");
            }
        }
    }
}
