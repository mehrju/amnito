using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Data
{
    public class Tbl_ServicesProvidersMap : EntityTypeConfiguration<Tbl_ServicesProviders>
    {
        public Tbl_ServicesProvidersMap()
        {
            ToTable("Tbl_ServicesProviders");
            HasKey(m => m.Id);
            Property(m => m.ServicesProviderName);
            Property(m => m.AgentName);
            Property(m => m.CategoryId);
            Property(m => m.ServiceTypeId);
            Property(m => m.IsActive);
            Property(m => m.MaxOrder);
            Property(m => m.MaxWeight);
            Property(m => m.MinWeight);
            Property(m => m.MaxTimeDeliver);
            Property(m => m.advancefreight);
            Property(m => m.freightforward);
            Property(m => m.cod);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
            Property(m => m.IsPishtaz);
            Property(m => m.IsSefareshi);
            Property(m => m.IsVIje);
            Property(m => m.IsNromal);
            Property(m => m.IsDroonOstani);
            Property(m => m.IsAdjoining);
            Property(m => m.IsNotAdjacent);
            Property(m => m.IsHeavyTransport);
            Property(m => m.IsForeign);
            Property(m => m.IsInCity);
            Property(m => m.IsTwoStep);
            Property(m => m.HasHagheMaghar);
            Property(m => m.MaxbillingamountCOD);
            Property(m => m.Maxheight);
            Property(m => m.Maxlength);
            Property(m => m.Maxwidth);






        }
    }
}
