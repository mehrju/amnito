using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface INotificationService
    {
        void SendSmsForPlaceOrder(Order order);
        void SendSmsByTemplate(Order _order, Shipment shipment, int notifTitle, bool isReciver);
        NotifTemplate getNotifTamplate(int NotifTypeId, Order order, Shipment shipment);
        bool IsSendedSmsToPostman(int shipmentId);
        void InsertNotifyPostMan(int shipmentId, string message, string mobile);
        bool _sendSms(string receiver, string msg);
        void sendSms(string msg, string receiver);
        void NotifyCollectShipment(int shipmentId);
        void SendSmsToSender_Reciver(int orderId, IExtendedShipmentService _extendedShipmentService);
        void NotifyPostSupervisor(Order order, int serviceId, IExtendedShipmentService _extendedShipmentService);
        void sendSmsPostAdminForNewOrder(string orderIds, IExtendedShipmentService _extendedShipmentService);
        void NotifyForCancel(int orderId, IExtendedShipmentService _extendedShipmentService);
        List<NotifItemsModel> getNotifItem(int NotifTypeId);
        NotifItemsModel IntiNotifModel(int NotifTypeId);
        bool SaveNotifItems(NotifItemsModel model, out string strError);
        PagedList<Tbl_PopupNotification> GetPopupNotifications(int page, int pageSize);
        List<NotifTitleModel> getNofitTitleList();
        void updateNotifTitle(List<int> notifitemIds);
        bool HasOrderSendingSmsRequest(Order order);
        bool IsSmsSendedForThis(NotifTitle notifTitle, int orderId, int shipmentId);
        void SavePopupNotification(PopupNotificationModel model);
        PopupNotificationModel GetPopupNotificationById(int id);
        PopupNotificationModel GetLatestNotification(int[] oldNotifs);
    }
}
