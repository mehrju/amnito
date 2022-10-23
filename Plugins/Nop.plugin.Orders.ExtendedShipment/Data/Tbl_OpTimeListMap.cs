using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_OpTimeListMap : EntityTypeConfiguration<Tbl_OpTimeList>
    {
        public Tbl_OpTimeListMap()
        {
            ToTable("Tbl_OpTimeList");
            HasKey(m => m.Id);
        }
    }
}
