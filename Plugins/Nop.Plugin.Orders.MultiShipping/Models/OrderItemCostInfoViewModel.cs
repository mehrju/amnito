using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class OrderItemCostInfoViewModel
    {
        public int AttrId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Price { get; set; }
    }
}
