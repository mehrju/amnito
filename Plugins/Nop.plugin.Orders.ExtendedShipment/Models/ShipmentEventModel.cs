using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Enums;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentEventModel : BaseEntity
    {
        public string ShipmentEventId { get; set; }
        public string ShipmentEventName { get; set; }

    }
}
