using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_AffiliateToCustomerMap : EntityTypeConfiguration<Tbl_AffiliateToCustomer>
    {
        public Tbl_AffiliateToCustomerMap()
        {
            ToTable("Tbl_AffiliateToCustomer");

            HasKey(m => m.Id);
            Property(m => m.CustomerId).IsRequired();
            Property(m => m.AffiliateId).IsRequired();
            Property(m => m.registerUserId).IsRequired();
            Property(m => m.registerDate).IsRequired();
            Property(m => m.IsActive).IsRequired();
            Property(m => m.LastDeActiveDate).IsOptional();
            Property(m => m.LastDeActiveUserId).IsOptional();
            Property(m => m.LastActiveDate).IsOptional();
            Property(m => m.LastActiveUser).IsOptional();


        }
    }
}
