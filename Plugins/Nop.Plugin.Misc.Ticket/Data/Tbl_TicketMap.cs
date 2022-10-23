using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_TicketMap : EntityTypeConfiguration<Tbl_Ticket>
    {
        public Tbl_TicketMap()
        {

            ToTable("Tbl_Ticket");

            HasKey(m => m.Id);
            Property(m => m.IdCustomer);
            Property(m => m.OrderCode).IsOptional();
            Property(m => m.TrackingCode).IsOptional();
            Property(m => m.StoreId);
            Property(m => m.ProrityId);
            Property(m => m.DepartmentId);
            Property(m => m.IdCategoryTicket);
            Property(m => m.ShowCustomer);
            Property(m => m.Issue);
            Property(m => m.IsActive);
            Property(m => m.Status);
            Property(m => m.DateInsert);
            Property(m => m.StaffIdAccept);
            Property(m => m.DateStaffAccept);
            Property(m => m.IdUserUpdate);
            Property(m => m.DateUpdate);
            Property(m => m.StaffIdLastAnswer);
            Property(m => m.DateStaffLastAnswer);
        }
    }
}
