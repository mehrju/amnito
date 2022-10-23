using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
   public class vm_SuccessOrder
    {
        public string NameCustomer { get; set; }
        public string OrderCode { get; set; }
        public string TrackingCode { get; set; }
        public bool StateTrackingCode { get; set; }
        public bool IsForeign { get; set; }

    }
}
