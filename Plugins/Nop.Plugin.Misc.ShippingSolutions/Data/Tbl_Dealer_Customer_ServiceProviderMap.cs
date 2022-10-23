using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_Dealer_Customer_ServiceProviderMap : EntityTypeConfiguration<Tbl_Dealer_Customer_ServiceProvider>
    {
        public Tbl_Dealer_Customer_ServiceProviderMap()
        {
            ToTable("Tbl_Dealer_Customer_ServiceProvider");

            HasKey(m => m.Id);
            Property(m => m.TypeUser);
            Property(m => m.CustomerId).IsOptional();
            Property(m => m.DealerId).IsOptional();
            Property(m => m.ProviderId);
            Property(m => m.MaxCountpackage);
            Property(m => m.MaxWeight);
            Property(m => m.StateNegative_credit_amount);
            Property(m => m.Negative_credit_amount);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
            Property(m => m.StateApplyPricingPolicy).IsOptional();


            this.HasRequired(i => i.Provider)
          .WithMany(s => s.Dealer_Customer_ServiceProvider)
          .HasForeignKey(i => i.ProviderId);

        }
    }
}
