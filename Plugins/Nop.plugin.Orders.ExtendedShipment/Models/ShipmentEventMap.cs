using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentEventMap: EntityTypeConfiguration<ShipmentEventModel>
    {
        public ShipmentEventMap()
        {
            ToTable("Tb_ShipmentEvent");
            HasKey(p => p.Id);
            Property(p => p.ShipmentEventId);
            Property(p => p.ShipmentEventName);
        }
    }
}
