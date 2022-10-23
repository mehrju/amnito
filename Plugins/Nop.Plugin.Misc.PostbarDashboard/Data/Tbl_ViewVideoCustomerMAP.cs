using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Data
{
    public class Tbl_ViewVideoCustomerMAP : EntityTypeConfiguration<Tbl_ViewVideoCustomer>
    {
        public Tbl_ViewVideoCustomerMAP()
        {
            ToTable("Tbl_ViewVideoCustomer");
            HasKey(m => m.Id);
            Property(m => m.CustomerId);
            Property(m => m.DateView);
            Property(m => m.IsActive);
            Property(m => m.IPCustomer);

        }
    }
}
