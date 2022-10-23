using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class ServiceInfoModel
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public string SLA { get; set; }
        public int Price { get; set; }
        public string _Price { get; set; }
        public bool IsCod { get; set; }
    }
}
