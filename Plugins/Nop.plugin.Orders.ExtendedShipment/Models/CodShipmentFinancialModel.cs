using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CodShipmentFinancialModel:BaseEntity
    {
        public int shipmentId { get; set; }
        public string VairzCode { get; set; }
        public DateTime VarizDate { get; set; }
        public int EngAndKalaPrice { get; set; }
        public int SumFraction { get; set; }
        public int Tax { get; set; }

    }
}
