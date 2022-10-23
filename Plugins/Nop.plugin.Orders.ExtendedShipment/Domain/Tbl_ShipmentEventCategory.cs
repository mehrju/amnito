using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_ShipmentEventCategory : BaseEntity
    {
        public string ShipmentEventId { get; set; }
        public int CategoryId { get; set; }
    }
}
