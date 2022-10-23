using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_CarriersTarofMap : EntityTypeConfiguration<Tbl_CarriersTarof>
    {
        public Tbl_CarriersTarofMap()
        {
            ToTable("Tbl_CarriersTarof");
            HasKey(m => m.Id);
            Property(m => m._id);
            Property(m => m.title);
        }
    }
}
