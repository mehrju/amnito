using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_Address_LatLongMap : EntityTypeConfiguration<Tbl_Address_LatLong>
    {
        public Tbl_Address_LatLongMap()
        {
            ToTable("Tbl_Address_LatLong");
            HasKey(m => m.Id);
            Property(m => m.AddressId);
            Property(m => m.Lat);
            Property(m => m.Long);

        }
    }
}
