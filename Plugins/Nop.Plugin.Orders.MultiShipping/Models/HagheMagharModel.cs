using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class HagheMagharModel : BaseEntity
    {
        public int OrderItemId { get; set; }
        public int HagheMagharPrice { get; set; }
        public int ShipmentHagheMaghr { get; set; }
    }
}
