using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_CategoryTicketMAP : EntityTypeConfiguration<Domain.Tbl_CategoryTicket>
    {
        public Tbl_CategoryTicketMAP()
        {
            ToTable("Tbl_CategoryTicket");

            HasKey(m => m.Id);
            Property(m => m.NameCategoryTicket);
            Property(m => m.DepartmentId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();

            this.HasRequired(i => i.Department)
           .WithMany(s => s.CategoryTicket)
           .HasForeignKey(i => i.DepartmentId);
        }
    }
}
