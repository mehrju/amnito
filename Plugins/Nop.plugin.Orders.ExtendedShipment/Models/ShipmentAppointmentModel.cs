using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentAppointmentModel:BaseEntity
    {
        public int ShipmentId { get; set; }
        public bool IsAutoPersuitCode { get; set; }
        public int PostManId { get; set; }
        public int PostAdminId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Barcode { get; set; }
        public bool IsDefrentWeight { get; set; }
        public DateTime? DataCollect { get; set; }
        public int? CodCost { get; set; }
        public int? CodBmValue { get; set; }
    }
}
