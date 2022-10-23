using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class PackingRequestModel
    {
        public int ShipmentId { get; set; }
        public string KartonSizeItemName { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int OrderId { get; set; }
        public string CustomerPhoneNumber { get; set; }
    }
}
