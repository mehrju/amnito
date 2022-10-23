using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_ParcelTypeSkyBlueMap : EntityTypeConfiguration<Tbl_ParcelTypeSkyBlue>
    {
        public Tbl_ParcelTypeSkyBlueMap()
        {
            ToTable("Tbl_ParcelTypeSkyBlue");
            HasKey(m => m.Id);
            Property(m => m.Code);
            Property(m => m.Name);
        }
    }
}
