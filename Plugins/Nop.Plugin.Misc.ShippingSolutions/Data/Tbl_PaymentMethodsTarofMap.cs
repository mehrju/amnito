using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
   public class Tbl_PaymentMethodsTarofMap : EntityTypeConfiguration<Tbl_PaymentMethodsTarof>
    {
        public Tbl_PaymentMethodsTarofMap()
        {
            ToTable("Tbl_PaymentMethodsTarof");
            HasKey(m => m.Id);
            Property(m => m._id);
            Property(m => m.title);
        }
    }
}
