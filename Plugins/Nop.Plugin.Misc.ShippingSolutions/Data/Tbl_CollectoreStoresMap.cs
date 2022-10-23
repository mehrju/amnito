using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    class Tbl_CollectoreStoresMap : EntityTypeConfiguration<Tbl_CollectoreStores>
    {
        public Tbl_CollectoreStoresMap()
        {
            ToTable("Tbl_CollectoreStores");

            HasKey(m => m.Id);
            Property(m => m.CollectorId);
            Property(m => m.StoreId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();


            this.HasRequired(i => i.Collector)
              .WithMany(s => s.CollectoreStores)
              .HasForeignKey(i => i.CollectorId);
        }
    }
}
