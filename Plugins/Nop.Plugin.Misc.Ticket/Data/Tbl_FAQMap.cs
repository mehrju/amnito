using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_FAQMap : EntityTypeConfiguration<Domain.Tbl_FAQ>
    {
        public Tbl_FAQMap()
        {
            ToTable("Tbl_FAQ");

            HasKey(m => m.Id);
            Property(m => m.Question);
            Property(m => m.Answer);
            Property(m => m.IdCategory);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();

            this.HasRequired(i => i.FAQCategory)
           .WithMany(s => s.tbl_FAQs)
           .HasForeignKey(i => i.IdCategory);
        }
    }
}
