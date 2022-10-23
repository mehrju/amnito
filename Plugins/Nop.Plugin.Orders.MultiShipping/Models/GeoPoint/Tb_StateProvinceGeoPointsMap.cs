using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.GeoPoint
{
    public class Tb_StateProvinceGeoPointsMap : EntityTypeConfiguration<Tb_StateProvinceGeoPoints>
    {
        public Tb_StateProvinceGeoPointsMap()
        {
            ToTable("Tb_StateProvinceGeoPoints");
            HasKey(m => m.Id);
            Property(m => m.StateProvinceId);
            Property(m => m.Lat);
            Property(m => m.Lon);

        }
    }
}
