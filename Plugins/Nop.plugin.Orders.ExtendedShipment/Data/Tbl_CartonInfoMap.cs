using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_CartonInfoMap : EntityTypeConfiguration<Tbl_CartonInfo>
    {
        public Tbl_CartonInfoMap()
        {
            ToTable("Tbl_CartonInfo");
            HasKey(p => p.Id);
        }
    }
}
