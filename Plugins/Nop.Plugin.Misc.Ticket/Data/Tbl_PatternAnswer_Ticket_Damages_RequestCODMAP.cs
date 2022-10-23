using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_PatternAnswer_Ticket_Damages_RequestCODMAP : EntityTypeConfiguration<Domain.Tbl_PatternAnswer_Ticket_Damages_RequestCOD>
    {
        public Tbl_PatternAnswer_Ticket_Damages_RequestCODMAP()
        {
            ToTable("Tbl_PatternAnswer_Ticket_Damages_RequestCOD");

            HasKey(m => m.Id);
            Property(m => m.TitlePatternAnswer);
            Property(m => m.DescriptipnPatternAnswer);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
