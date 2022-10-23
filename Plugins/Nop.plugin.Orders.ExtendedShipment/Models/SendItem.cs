using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class senditem
    {
        public Listitem[] Listitem { get; set; }
    }
    public class Listitem
    {
        public int _Price { get; set; }
        public int _NewPrice { get; set; }
        public int _OrderItemId { get; set; }
    }


}
