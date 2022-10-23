using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
   public interface ITrackingUbaarOrder_Service
    {
        bool Insert(int _OrderId, int _OrderItemId, int _Status, string _Des, int _Price, int _NewPrice);
    }
}
