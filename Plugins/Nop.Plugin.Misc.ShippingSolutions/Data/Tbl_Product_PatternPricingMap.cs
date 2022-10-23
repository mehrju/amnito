using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_Product_PatternPricingMap : EntityTypeConfiguration<Tbl_Product_PatternPricing>
    {
        public Tbl_Product_PatternPricingMap()
        {
            ToTable("Tbl_Product_PatternPricing");

            HasKey(m => m.Id);
            Property(m => m.IdProduct);
            //Property(m => m.Price);
            Property(m => m.IdPatternPricing);
            Property(m => m.StateApplyPricingPolicy);
            Property(m => m.StateClaculateMonth);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
            Property(m => m.StateApplyPricingPolicy);
        }
    }
}
