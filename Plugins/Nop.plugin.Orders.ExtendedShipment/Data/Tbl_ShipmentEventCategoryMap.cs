using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_ShipmentEventCategoryMap : EntityTypeConfiguration<Tbl_ShipmentEventCategory>
    {
        public Tbl_ShipmentEventCategoryMap()
        {
            ToTable("Tbl_ShipmentEventCategory");
            HasKey(p=>p.Id);
        }
    }
}
