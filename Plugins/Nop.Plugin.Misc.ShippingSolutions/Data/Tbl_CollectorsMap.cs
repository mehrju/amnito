using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_CollectorsMap : EntityTypeConfiguration<Tbl_Collectors>
    {
        public Tbl_CollectorsMap()
        {
            ToTable("Tbl_Collectors");
            HasKey(m => m.Id);
            Property(m => m.CollectorName);
            Property(m => m.UserId);
            Property(m => m.IsActive);
            Property(m => m.MaxPath);
            Property(m => m.MaxWeight);
            Property(m => m.MinWeight);
            //Property(m => m.advancefreight);
            //Property(m => m.freightforward);
            //Property(m => m.cod);
            //Property(m => m.DateInsert);
            //Property(m => m.DateUpdate).IsOptional();
            //Property(m => m.IdUserInsert);
            //Property(m => m.IdUserUpdate).IsOptional();
            //Property(m => m.IsPishtaz);
            //Property(m => m.IsSefareshi);
            //Property(m => m.IsVIje);
            //Property(m => m.IsNromal);
            //Property(m => m.IsDroonOstani);
            //Property(m => m.IsAdjoining);
            //Property(m => m.IsNotAdjacent);
            //Property(m => m.IsHeavyTransport);
            //Property(m => m.IsForeign);
            //Property(m => m.IsInCity);
            //Property(m => m.IsTwoStep);
            //Property(m => m.HasHagheMaghar);

        }
    }
}
