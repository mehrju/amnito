using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_PatternPricingPolicyMap : EntityTypeConfiguration<Tbl_PatternPricingPolicy>
    {
        public Tbl_PatternPricingPolicyMap()
        {
            ToTable("Tbl_PatternPricingPolicy");
            HasKey(m => m.Id);
            Property(m => m.CategoryId);
            Property(m => m.Name);
            Property(m => m.MinCount);
            Property(m => m.MaxCount);
            Property(m => m.MinWeight);
            Property(m => m.MaxWeight);
            Property(m => m.Percent);
            Property(m => m.Mablagh);
            Property(m => m.Tashim);
            Property(m => m.PercentTashim);
            Property(m => m.IsActive);
            Property(m => m.IdParent);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
