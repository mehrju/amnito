using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_PricingPolicyMap : EntityTypeConfiguration<Tbl_PricingPolicy>
    {
        public Tbl_PricingPolicyMap()
        {
            ToTable("Tbl_PricingPolicy");

            HasKey(m => m.Id);
            Property(m => m.ProviderId);
            Property(m => m.Dealer_Customer_Id);
            Property(m => m.CountryId);
            Property(m => m.MinWeight);
            Property(m => m.MaxWeight);
            Property(m => m.Percent);
            Property(m => m.Mablagh);
            Property(m => m.Tashim);
            Property(m => m.PercentTashim);

            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();

            //this.HasRequired(i => i.Provider)
            //    .WithMany(s => s.PricingPolicyServiceProvider)
            //    .HasForeignKey(i => i.ProviderId);

            //this.HasRequired(i => i.DealerCustomer)
            //               .WithMany(s => s.PricingPolicyCustomerDealer)
            //               .HasForeignKey(i => i.Dealer_Customer_Id);

        }
    }
}
