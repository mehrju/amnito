using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.GeoPoint
{
    public class Tb_CountryGeoPointsMap : EntityTypeConfiguration<Tb_CountryGeoPoints>
    {
        public Tb_CountryGeoPointsMap()
        {
            ToTable("Tb_CountryGeoPoints");
            HasKey(m => m.Id);
            Property(m => m.CountryId);
            Property(m => m.Lat);
            Property(m => m.Lon);
        }
    }
}
