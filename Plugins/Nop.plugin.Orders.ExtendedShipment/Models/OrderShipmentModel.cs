using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class OrderShipmentModel
    {
        public int ShipmentId { get; set; }
        public string FullName { get; set; }
        public string address { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippedDateUtc { get; set; }
        public string DeliveryDateUtc { get; set; }
        public string ProductName { get; set; }
    }
}
