using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_Ticket_PriorityMap : EntityTypeConfiguration<Tbl_Ticket_Priority>
    {
        public Tbl_Ticket_PriorityMap()
        {
            ToTable("Tbl_Ticket_Priority");

            HasKey(m => m.Id);
            Property(m => m.Name);
            Property(m => m.Sort);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
