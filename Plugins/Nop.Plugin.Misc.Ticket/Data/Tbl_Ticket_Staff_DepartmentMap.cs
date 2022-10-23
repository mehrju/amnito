using Nop.Plugin.Misc.Ticket.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_Ticket_Staff_DepartmentMap : EntityTypeConfiguration<Tbl_Ticket_Staff_Department>
    {
        public Tbl_Ticket_Staff_DepartmentMap()
        {
            ToTable("Tbl_Ticket_Staff_Department");

            HasKey(m => m.Id);
            Property(m => m.IdDepartment);
            Property(m => m.UserId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();

            this.HasRequired(i => i.Department)
             .WithMany(s => s.Staff_Departments)
             .HasForeignKey(i => i.IdDepartment);


        }
    }
}
