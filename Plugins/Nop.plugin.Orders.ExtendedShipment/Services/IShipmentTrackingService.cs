using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IShipmentTrackingService
    {
        List<TrackingInfo> getLastShipmentTracking(string trackingNumber, int orderId, string mobileNo,int CustomerId, int IdServiceProvider,out string strError);
        int GetShipmentStatistic(DateTime startDate, DateTime endDate);
        DataTable getNoramlShipmentTracking(Shipment shipment);
        bool RegisterLastShipmentStatus(int shipmentID, bool isCod,out string result);
        void Log(string header, string Message);

        List<QualityControlModel> getQualityControl(int orderId, string trackingNumber, int countryId, int stateId
            , int orderState, DateTime? DateFrom, DateTime? DateTo, int PageSize, int PageIndex, out int count);

        bool HasTrackingRecourd(int orderId);
        List<OrderShipmentInfo> getAllShipmentByOrderId(int orderId);
        OrderShipmentInfoDetails getShipmentDetails(string TrackingNumber, int OrderId, int ShipmentId, out string error);
        List<WeightCategoryModel> GetWightCategories();
        void InsertTracking(int shipmentId, OrderShipmentStatusEnum status, string des = "");
        void InsertTracking(int shipmentId, int status, string des);
    }
}
