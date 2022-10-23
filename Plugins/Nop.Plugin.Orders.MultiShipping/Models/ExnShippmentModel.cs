using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class ExnShippmentModel : BaseEntity
    {
        [NotMapped]
        public Shipment shipment { get; set; }
        public int ShipmentId { get; set; }
        public int? ShipmentTempId { get; set; }
        public int ShippmentAddressId { get; set; }
        public string ShippmentMethod { get; set; }
    }
}
