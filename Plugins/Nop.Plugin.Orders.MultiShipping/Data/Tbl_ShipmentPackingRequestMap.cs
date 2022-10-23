using Nop.Plugin.Orders.MultiShipping.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Data
{
    public class Tbl_ShipmentPackingRequestMap : EntityTypeConfiguration<Tbl_ShipmentPackingRequest>
    {
        public Tbl_ShipmentPackingRequestMap()
        {
            ToTable("Tbl_ShipmentPackingRequest");
            HasKey(p => p.Id);
            Property(p => p.Height);
            Property(p => p.Length);
            Property(p => p.Width);
            Property(p => p.IsSmsSent);
            Property(p => p.KartonSizeItemName);
            Property(p => p.ShipmentId);
            Property(p => p.Status);
            Property(p => p.CustomerPhone);
        }
    }
}
