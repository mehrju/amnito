using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_PhoneOrder_OrderMap : EntityTypeConfiguration<Tbl_PhoneOrder_Order>
    {
        public Tbl_PhoneOrder_OrderMap()
        {
            ToTable("Tbl_PhoneOrder_Order");
            HasKey(m => m.Id);
        }
    }
}
