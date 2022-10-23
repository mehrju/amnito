using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CODShipmentEventMap: EntityTypeConfiguration<CODShipmentEventModel>
    {
        public CODShipmentEventMap()
        {
            ToTable("Tb_CODShipmentEvent");
            HasKey(p => p.Id);
            Property(p => p.EventCode);
            Property(p => p.EventName);
        }
    }
}
