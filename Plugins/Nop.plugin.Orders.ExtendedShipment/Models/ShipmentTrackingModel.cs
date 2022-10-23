using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentTrackingModel : BaseEntity
    {
        public int ShipmentId { get; set; } = 0;
        public DateTime LastShipmentEventDate { get; set; }
        public int ShipmentEventId { get; set; }
        public int CODShipmentEventId { get; set; }
        [NotMapped]
        public string EventName { get; set; }
        public string ShipmenMasterEventCode { get; set; }
        public string ShipmenEventDesc { get; set; }
        public int? CreateCustomerId { get; set; }
        public bool IsCurrent { get; set; }
    }
}
