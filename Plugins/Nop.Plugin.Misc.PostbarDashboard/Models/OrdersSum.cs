using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class OrdersSum
    {
        public long? OrderTotal { get; set; }
        public long? DiscountSum { get; set; }
    }
}
