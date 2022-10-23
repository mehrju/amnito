using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Data
{
    public class Tbl_OptimeListDetailesMap : EntityTypeConfiguration<Tbl_OptimeListDetailes>
    {
        public Tbl_OptimeListDetailesMap()
        {
            ToTable("Tbl_OptimeListDetailes");
            HasKey(m => m.Id);
        }
    }
}
