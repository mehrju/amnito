using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
   public class Tbl_LogSMSMap : EntityTypeConfiguration<Domain.Tbl_LogSMS>
    {
        public Tbl_LogSMSMap()
        {
            ToTable("Tbl_LogSMS");

            HasKey(m => m.Id);
            Property(m => m.Type);
            Property(m => m.Mobile);
            Property(m => m.DateSend);
            Property(m => m.TextMessage);
            Property(m => m.StoreId);
            Property(m => m.Status);

        }
    }
}
