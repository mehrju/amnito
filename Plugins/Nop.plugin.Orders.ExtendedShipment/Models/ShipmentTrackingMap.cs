using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentTrackingMap : EntityTypeConfiguration<ShipmentTrackingModel>
    {
        public ShipmentTrackingMap()
        {
            ToTable("ShipmentTracking");
            HasKey(p => p.Id);
            Property(p => p.LastShipmentEventDate);
            Property(p => p.ShipmentEventId);
            Property(p => p.CODShipmentEventId);
            Property(p => p.ShipmenMasterEventCode);
            Property(p => p.ShipmenEventDesc);
        }
    }
}
