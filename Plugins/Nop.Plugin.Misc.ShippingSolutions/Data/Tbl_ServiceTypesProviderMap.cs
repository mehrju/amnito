using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_ServiceTypesProviderMap : EntityTypeConfiguration<Tbl_ServiceTypesProvider>
    {
        public Tbl_ServiceTypesProviderMap()
        {
            ToTable("Tbl_ServiceTypesProvider");

            HasKey(m => m.Id);
            Property(m => m.ProviderId);
            Property(m => m.ServiceTypeId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
           

          

            //this.HasRequired(i => i.ServiceType)
            //   .WithMany(s => s.ServiceTypesProvider)
            //   .HasForeignKey(i => i.ServiceTypeId);

        }
    }
}
