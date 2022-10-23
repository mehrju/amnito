using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_TrackingForeignOrderMap : EntityTypeConfiguration<Tbl_TrackingForeignOrder>
    {
        public Tbl_TrackingForeignOrderMap()
        {
            ToTable("Tbl_TrackingForeignOrder");
            HasKey(m => m.Id);
            Property(m => m.OrderId);
            Property(m => m.ShipingId);
            Property(m => m.Status);
            Property(m => m.Description);
            Property(m => m.IP);
            Property(m => m.Mablagh);
            Property(m => m.IsPay);
            Property(m => m.DateInsert);
            Property(m => m.IdUserInsert);
        }
    }
}
