using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_ProviderStoresMap : EntityTypeConfiguration<Tbl_ProviderStores>
    {
        public Tbl_ProviderStoresMap()
        {
            ToTable("Tbl_ProviderStores");

            HasKey(m => m.Id);
            Property(m => m.ProviderId);
            Property(m => m.StoreId);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
           

            this.HasRequired(i => i.Provider)
              .WithMany(s => s.ProviderStores)
              .HasForeignKey(i => i.ProviderId);
        }
    }
}
