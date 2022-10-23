using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class OrderStatusTrackingModel:BaseEntity
    {
        public int orderId { get; set; }
        public int OrderstatusId{ get; set; }
        public DateTime chageDate { get; set; }
        public int? CustomerId { get; set; }
        public string ClientIp { get; set; }

    }
}
