using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CODShipmentEventModel:BaseEntity
    {
        public string EventCode { get; set; }
        public string EventName { get; set; }
    }
}
