using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class SnapboxWebhook : ISnapboxWebhook
    {
        private readonly IOrderService _orderService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly INotificationService _notificationService;
        private readonly IShipmentService _shipmentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IGenericAttributeService _genericAttributeService;

        public SnapboxWebhook(IOrderService orderService
            , IExtendedShipmentService extendedShipmentService
            , INotificationService notificationService
            , IShipmentService shipmentService
            , IOrderProcessingService orderProcessingService
            , IGenericAttributeService genericAttributeService)
        {
            _genericAttributeService = genericAttributeService;
            _orderService = orderService;
            _extendedShipmentService = extendedShipmentService;
            _notificationService = notificationService;
            _shipmentService = shipmentService;
            _orderProcessingService = orderProcessingService;
        }
        public void SnapBoxAccepted(int orderId,string SnappOrderId,string bikerPhoneNumber,string bikerName)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                _genericAttributeService.SaveAttribute<string>(order, "SnappboxOrderId", SnappOrderId, order.StoreId);
                string message = $@"امنیتو اسنپ: سفارش ما توسط {bikerName} با شماره تماس {bikerPhoneNumber} پذیرفته شد."
                + "\r\n" 
                + "جهت پرداخت از لینک زیر استفاده کنید"
                +"\r\n"
                + "https://postex.ir/OrderPayment?orderId=" + orderId;
                _extendedShipmentService.SavePostCoordination(orderId, "پذیرش درخوست از سوی اسنپ باکس");
                _notificationService._sendSms(order.BillingAddress.PhoneNumber, message);
                common.InsertOrderNote("شماره تماس سفیر اسنپ:" + bikerPhoneNumber, orderId);
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapBoxAccepted");
                common.LogException(ex);
            }

        }
        public void SnapboxARRIVIED(int orderId, string bycerMobileNo)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                string msgForPardakht = "";
                if (order.PaymentStatus != Nop.Core.Domain.Payments.PaymentStatus.Paid)
                {
                    string message = $@"سفارش مشاره {order.Id}  مربوط به اسنپ باکس می باشد و کاربر پرداخت را انجام نداده (فرستنده:{order.BillingAddress.PhoneNumber} "
                        + "شماره تماس سفیر اسنپ: " + bycerMobileNo;
                    _notificationService._sendSms("09129427467", message);
                    msgForPardakht = "لطفا قبل از تحویل کالا به سفیر، پرداخت را انجام دهید";
                }
                string msg = "سفیر حمل کالای شما به محل رسید";
                msg += msgForPardakht != "" ?"\r\n"+ msgForPardakht : "";
                _notificationService._sendSms(order.BillingAddress.PhoneNumber, msg);
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxARRIVIED");
                common.LogException(ex);
            }
        }
        public void SnapboxPickup(int orderId, string bycerMobileNo)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order.PaymentStatus != Nop.Core.Domain.Payments.PaymentStatus.Paid)
                {
                    string message = $@"سفارش مشاره {order.Id}  مربوط به اسنپ باکس می باشد و کاربر پرداخت را انجام نداده (فرستنده:{order.BillingAddress.PhoneNumber} "
                         + "شماره تماس سفیر اسنپ: " + bycerMobileNo;
                    _notificationService._sendSms("09129427467", message);
                }
                var shipment = order.Shipments.First();
                shipment.ShippedDateUtc = DateTime.Now.ToUniversalTime();
                _shipmentService.UpdateShipment(shipment);
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxPickup");
                common.LogException(ex);
            }
        }
        public void SnapboxDeliver(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                var shipment = order.Shipments.First();
                shipment.DeliveryDateUtc = DateTime.Now.ToUniversalTime();
                _shipmentService.UpdateShipment(shipment);
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxDeliver");
                common.LogException(ex);
            }
        }
        public void SnapboxBikerCancel(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return;
                if (order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                    return;
                common.InsertOrderNote("CancelFromSnap", order.Id);
                _orderProcessingService.CancelOrder(order, true);
                
            }
            catch (Exception ex)
            {
                common.Log("webhook", "SnapboxBikerCancel");
                common.LogException(ex);
            }
        }
    }
}
