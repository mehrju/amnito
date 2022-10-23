using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_Ticket_DepartmentMap : EntityTypeConfiguration<Tbl_Ticket_Department>
    {
        public Tbl_Ticket_DepartmentMap()
        {
            ToTable("Tbl_Ticket_Department");

            HasKey(m => m.Id);
            Property(m => m.Name);
            Property(m => m.StoreId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
