using Nop.Core;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class MultiShipmentModel : BaseEntity
    {
        [NotMapped]
        public Shipment shipment { get; set; }
        public int ShipmentId { get; set; }
        public int ShipmentAddressId { get; set; }
        public int ProductId { get; set; }
    }
}
