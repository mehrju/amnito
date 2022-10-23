using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_CancelReason_OrderMap : EntityTypeConfiguration<Tbl_CancelReason_Order>
    {
        public Tbl_CancelReason_OrderMap()
        {
            ToTable("Tbl_CancelReason_Order");

            HasKey(m => m.Id);
            Property(m => m.id_Description);
            Property(m => m.OrderId);
            Property(m => m.DateInsert);
            Property(m => m.IdUserInsert);
            Property(m => m.RefoundItem);

        }
    }
}
