using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_WorkingTimeMap : EntityTypeConfiguration<Tbl_WorkingTime>
    {
        public Tbl_WorkingTimeMap()
        {

            ToTable("Tbl_WorkingTime");
            
            HasKey(m => m.Id);
            Property(m => m.OfficeId);
            Property(m => m.DayName);
            Property(m => m.StartTime).IsOptional();
            Property(m => m.EndTime).IsOptional();
            //Property(m => m.IsActive);
            //Property(m => m.DateInsert);
            //Property(m => m.DateUpdate).IsOptional();
            //Property(m => m.IdUserInsert);
            //Property(m => m.IdUserUpdate).IsOptional();

            this.HasRequired(i => i.Offices)
               .WithMany(s => s.WorkingTimes)
               .HasForeignKey(i => i.OfficeId);

        }
    }
}
