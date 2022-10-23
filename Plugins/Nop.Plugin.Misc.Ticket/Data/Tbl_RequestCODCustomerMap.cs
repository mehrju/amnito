using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_RequestCODCustomerMap : EntityTypeConfiguration<Tbl_RequestCODCustomer>
    {
        public Tbl_RequestCODCustomerMap()
        {
            ToTable("Tbl_RequestCODCustomer");

            HasKey(m => m.Id);
            Property(m => m.IdCustomer);
            Property(m => m.Fname);
            Property(m => m.StoreId);
            Property(m => m.Lname);
            Property(m => m.NatinolCode);
            Property(m => m.UrlFile);
            Property(m => m.Address);
            Property(m => m.BusinessType);
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
            Property(m => m.Username);
            Property(m => m.CodePosti);
            Property(m => m.IdTicket);

        }
    }
}
