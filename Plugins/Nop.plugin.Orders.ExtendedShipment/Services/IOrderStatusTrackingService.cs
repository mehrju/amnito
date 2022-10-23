using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IOrderStatusTrackingService
    {
        void Insert(Order order);
        void Insert(int orderId, int orderStatusId);
        bool IsFirstOrder(int orderId);
    }
}
