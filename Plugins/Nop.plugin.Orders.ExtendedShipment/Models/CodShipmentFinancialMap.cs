using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CodShipmentFinancialMap: EntityTypeConfiguration<CodShipmentFinancialModel>
    {
        public CodShipmentFinancialMap()
        {
            ToTable("CodShipmentFinancial");
            HasKey(p => p.Id);
            Property(p => p.shipmentId);
            Property(p => p.EngAndKalaPrice);
            Property(p => p.SumFraction);
            Property(p => p.Tax);
            Property(p => p.VairzCode);
            Property(p => p.VarizDate);
        }
    }
}
