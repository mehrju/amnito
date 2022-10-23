using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tb_RelatedOrdersMap : EntityTypeConfiguration<Tb_RelatedOrders>
    {
        public Tb_RelatedOrdersMap()
        {
            ToTable("Tb_RelatedOrders");
            HasKey(p => p.Id);
            Property(p => p.ParentOrderId);
            Property(p => p.ChildOrderId);
            Property(p => p.ChildOrderPrice);

        }
    }
}
