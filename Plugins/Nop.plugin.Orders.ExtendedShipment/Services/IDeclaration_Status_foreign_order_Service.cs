using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
   public interface IDeclaration_Status_foreign_order_Service
    {
        bool IsOrderConfirm(int orderId);
        bool Insert_to_TrackingForeign(int _OrderId, int _Status, string _Des, int _Mablagh, int _ShipingId);
        List<Tbl_TrackingForeignOrder> GetTbl_TrackingForeignOrders(int _OrderId, int _ShipingId);
    }
}
