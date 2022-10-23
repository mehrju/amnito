using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_CountryISO3166Map : EntityTypeConfiguration<Tbl_CountryISO3166>
    {
        public Tbl_CountryISO3166Map()
        {
            ToTable("Tbl_CountryISO3166");
            HasKey(m => m.Id);
            Property(m => m.Name_E);
            Property(m => m.Name_F);
            Property(m => m.Alpha2Code);
            Property(m => m.NumericCode);
            Property(m => m.PDEId);
            Property(m => m.IsActive);

        }
    }
}
