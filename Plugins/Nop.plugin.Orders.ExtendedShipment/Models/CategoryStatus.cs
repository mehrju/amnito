using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CategoryStatus
    {
        public int StatusId { get; set; }
        public int[] CategoryIds { get; set; }
    }
}
