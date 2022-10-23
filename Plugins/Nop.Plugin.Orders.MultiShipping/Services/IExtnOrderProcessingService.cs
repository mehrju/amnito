using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Common;
using System.Collections.Generic;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Common;
using Nop.plugin.Orders.ExtendedShipment.Services;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface IExtnOrderProcessingService: IOrderProcessingService
    {
        List<string> EndOfOrderPlaced(Order order, int? registrationMethod = 1, int bulkorderId = 0, bool IsFromAp = false, bool IsFromSep = false);
        PlaceOrderResult PlaceOrderApi_Postkhone(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments, int? registrationMethod, int bulkorderId = 0);
        List<OrderDetails_ShippingModel> Get_OrderDetails_Shipping(int orderId, int pageSize, int PageIndex, out int count);
        PlaceOrderResult PlaceOrderWallet(ProcessPaymentRequest processPaymentRequest);
        void InsertOrderNote(string note, int orderId);
        bool HasDateCollect(Shipment shipment);
        PlaceOrderResult PlaceOrderApi(ProcessPaymentRequest processPaymentRequest,
            List<ExnShippmentModel> shipments = null, int? registrationMethod = null, int bulkorderId = 0);
        PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments = null);
        PlaceOrderResult PlaceOrderNewCheckOut(ProcessPaymentRequest processPaymentRequest, List<ExnShippmentModel> shipments, float? Senderlat, float? SenderLon,
            bool IsFromAp = false, bool IsFromSep = false);
        List<AddressModel> getOrderAddress(int orderId);
        bool IsMultiShippment(Order order);
        bool IsMultiShippment(int orderId);
        AddressModel getShippingAddres(int shipmentId);
        bool InsertCanceledShipment(Shipment shipment);
        int CheckAddressNeedHagheMaghar(Address address, int customerId, int ServiceId, int TotalWeight
            , bool isInTarheTraffic = false, int InComeCount = 0);
        PlaceOrderResult PlaceOrderCarton(ProcessPaymentRequest processPaymentRequest);
        
    }
}
