using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_DealerMap : EntityTypeConfiguration<Tbl_Dealer>
    {
        public Tbl_DealerMap()
        {
            ToTable("Tbl_Dealer");

            HasKey(m => m.Id);
            Property(m => m.Name);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
            Property(m => m.StateApplyPricingPolicy).IsOptional();


        }
    }
}
