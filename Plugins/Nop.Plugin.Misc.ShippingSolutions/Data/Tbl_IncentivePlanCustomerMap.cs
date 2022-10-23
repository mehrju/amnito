using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_IncentivePlanCustomerMap : EntityTypeConfiguration<Tbl_IncentivePlanCustomer>
    {
        public Tbl_IncentivePlanCustomerMap()
        {
            ToTable("Tbl_IncentivePlanCustomer");

            HasKey(m => m.Id);
            Property(m => m.IsActiveIncentivePlan).IsRequired();
            Property(m => m.IsAutomaticIncentivePlan).IsRequired();
            Property(m => m.DateInsert).IsRequired();
            Property(m => m.IdDiscountPlan).IsRequired();
            Property(m => m.IdUserInsert).IsRequired();
            Property(m => m.CustomerId).IsRequired();


        }
    }
}
