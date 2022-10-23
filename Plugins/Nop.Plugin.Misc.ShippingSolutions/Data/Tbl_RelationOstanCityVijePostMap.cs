using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_RelationOstanCityVijePostMap : EntityTypeConfiguration<Tbl_RelationOstanCityVijePost>
    {
        public Tbl_RelationOstanCityVijePostMap()
        {
            ToTable("Tbl_RelationOstanCityVijePost");
            HasKey(m => m.Id);
            Property(m => m.IdCity).IsOptional();
            Property(m => m.IdCountryDes);
            Property(m => m.IdCountryRegion);
            Property(m => m.Name);

        }
    }
}
