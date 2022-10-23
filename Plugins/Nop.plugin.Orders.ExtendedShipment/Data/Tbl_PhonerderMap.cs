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
    public class Tbl_PhoneOrderMap : EntityTypeConfiguration<Tbl_PhoneOrder>
    {
        public Tbl_PhoneOrderMap()
        {
            ToTable("Tbl_PhoneOrder");
            HasKey(m => m.Id);
        }
    }
}
