using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_CollectorOptimeUserMap : EntityTypeConfiguration<Tbl_CollectorOptimeUser>
    {
        public Tbl_CollectorOptimeUserMap()
        {
            ToTable("Tbl_CollectorOptimeUser");
        }
    }
}
