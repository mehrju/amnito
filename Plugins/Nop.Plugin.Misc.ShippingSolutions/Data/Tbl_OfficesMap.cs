using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_OfficesMap : EntityTypeConfiguration<Tbl_Offices>
    {
        public Tbl_OfficesMap()
        {
            ToTable("Tbl_Offices");

            HasKey(m => m.Id);
            Property(m => m.ProviderId);
            Property(m => m.TypeOffice);
            Property(m => m.ProviderId).IsOptional(); ;
            Property(m => m.CollectorId).IsOptional(); ;
            Property(m => m.WarehouseAddress).IsOptional();
            Property(m => m.WarehouseState);
            Property(m => m.AddressId).IsOptional();
            //Property(m => m.Lat);
            //Property(m => m.Long);
            Property(m => m.HolidaysState);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
            Property(m => m.StateProvinceId);
            //Property(m => m.IdCity).IsOptional();
            //Property(m => m.NameState).IsOptional();
            //Property(m => m.NameCity).IsOptional();




            //this.HasRequired(i => i.Provider)
            //   .WithMany(s => s.Offices)
            //   .HasForeignKey(i => i.ProviderId);

            //this.HasRequired(i => i.Collector)
            //     .WithMany(s => s.Offices)
            //     .HasForeignKey(i => i.CollectorId);


        }
    }
}
