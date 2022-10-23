using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_Ticket_DetailMap : EntityTypeConfiguration<Tbl_Ticket_Detail>
    {
        public Tbl_Ticket_DetailMap()
        {
            ToTable("Tbl_Ticket_Detail");

            HasKey(m => m.Id);
            Property(m => m.IdTicket);
            Property(m => m.Type);
            Property(m => m.StaffId);
            Property(m => m.DateInsert);
            Property(m => m.Description);
            Property(m => m.UrlFile1);
            Property(m => m.UrlFile2);
            Property(m => m.UrlFile3);

            this.HasRequired(i => i.Ticket)
            .WithMany(s => s.ticket_Details)
            .HasForeignKey(i => i.IdTicket);

        }
    }
}
