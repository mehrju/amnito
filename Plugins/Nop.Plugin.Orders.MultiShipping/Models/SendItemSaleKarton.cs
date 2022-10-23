using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class SendItemSaleKarton
    {
        public Listitem[] Listitem { get; set; }
    }
    public class Listitem
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
}
