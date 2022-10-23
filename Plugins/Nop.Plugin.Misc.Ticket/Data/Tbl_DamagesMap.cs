using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_DamagesMap : EntityTypeConfiguration<Tbl_Damages>
    {
        public Tbl_DamagesMap()
        {
            ToTable("Tbl_Damages");

            HasKey(m => m.Id);
            Property(m => m.IdCustomer);
            Property(m => m.TrackingCode);
            Property(m => m.StoreId);
            Property(m => m.NameGoods);
            Property(m => m.Berand);
            Property(m => m.Stock);
            Property(m => m.Price);
            Property(m => m.StoreId);
            Property(m => m.IsActive);
            Property(m => m.Status);
            Property(m => m.DateInsert);
            Property(m => m.StaffIdAccept);
            Property(m => m.DateStaffAccept);
            Property(m => m.IdUserUpdate);
            Property(m => m.DateUpdate);
            Property(m => m.StaffIdLastAnswer);
            Property(m => m.DateStaffLastAnswer);
            Property(m => m.IdTicket);

        }
    }
}
