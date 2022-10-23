using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_DiscountPlan_AgentCustomerMap : EntityTypeConfiguration<Tbl_DiscountPlan_AgentCustomer>
    {
        public Tbl_DiscountPlan_AgentCustomerMap()
        {
            ToTable("Tbl_DiscountPlan_AgentCustomer");

            HasKey(m => m.Id);
            Property(m => m.Name);
            Property(m => m.OfAmount);
            Property(m => m.UpAmount);
            Property(m => m.ExpireDay);
            Property(m => m.IsAgent);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
