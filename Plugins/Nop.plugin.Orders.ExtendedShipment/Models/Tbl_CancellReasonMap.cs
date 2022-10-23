using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    class Tbl_CancellReasonMap : EntityTypeConfiguration<Tbl_CancellReason>
    {
        public Tbl_CancellReasonMap()
        {
            ToTable("Tbl_CancellReason");

            HasKey(m => m.Id);
            Property(m => m.Description);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.IdUserInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserUpdate).IsOptional();

        }
    }
}
