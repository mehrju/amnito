using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_Damages_DetailMap : EntityTypeConfiguration<Tbl_Damages_Detail>
    {
        public Tbl_Damages_DetailMap()
        {
            ToTable("Tbl_Damages_Detail");

            HasKey(m => m.Id);
            Property(m => m.IdDamages);
            Property(m => m.Type);
            Property(m => m.StaffId);
            Property(m => m.DateInsert);
            Property(m => m.Description);
            Property(m => m.UrlFileCardMeli);
            Property(m => m.UrlFile1);
            Property(m => m.UrlFile2);
            Property(m => m.UrlFile3);
            Property(m => m.UrlFile4);
            this.HasRequired(i => i.Damages)
            .WithMany(s => s.Damages_Details)
            .HasForeignKey(i => i.IdDamages);
        }
    }
}
