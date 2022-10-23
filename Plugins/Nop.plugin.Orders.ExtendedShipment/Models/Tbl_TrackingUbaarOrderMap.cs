using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_TrackingUbaarOrderMap : EntityTypeConfiguration<Tbl_TrackingUbaarOrder>
    {
        public Tbl_TrackingUbaarOrderMap()
        {
            ToTable("Tbl_TrackingUbaarOrder");
            HasKey(m => m.Id);
            Property(m => m.OrderId);
            Property(m => m.OrderItemId);
            Property(m => m.Status);
            Property(m => m.Description);
            Property(m => m.IP);
            Property(m => m.price);
            Property(m => m.newprice);
            Property(m => m.IsPay);
            Property(m => m.DateInsert);
            Property(m => m.IdUserInsert);
        }
    }
}
