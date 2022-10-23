using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_SalesPartnersPercentMap : EntityTypeConfiguration<Tbl_SalesPartnersPercent>
    {
        public Tbl_SalesPartnersPercentMap()
        {
            ToTable("Tbl_SalesPartnersPercent");

            HasKey(m => m.Id);
            Property(m => m.Name).IsRequired();
            Property(m => m.OfDay).IsOptional();
            Property(m => m.UpDay).IsOptional();
            Property(m => m._Percent).IsRequired();
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
