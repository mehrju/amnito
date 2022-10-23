using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_CollectorsServiceProviderMap : EntityTypeConfiguration<Tbl_CollectorsServiceProvider>
    {
        public Tbl_CollectorsServiceProviderMap()
        {
            ToTable("Tbl_CollectorsServiceProvider");

            HasKey(m => m.Id);
            Property(m => m.CollectorId);
            Property(m => m.ProviderId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();


            this.HasRequired(i => i.Collector)
              .WithMany(s => s.CollectorsServiceProvider)
              .HasForeignKey(i => i.CollectorId);

            this.HasRequired(i => i.Provider)
             .WithMany(s => s.CollectorsServiceProvider)
             .HasForeignKey(i => i.ProviderId);
        }
    }
}
