using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_OstantarofMap : EntityTypeConfiguration<Tbl_Ostantarof>
    {
        public Tbl_OstantarofMap()
        {
            ToTable("Tbl_Ostantarof");
            HasKey(m => m.Id);
            Property(m => m._id);
            Property(m => m.title);
        }
    }
}
