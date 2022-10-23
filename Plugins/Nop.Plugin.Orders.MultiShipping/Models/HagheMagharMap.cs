using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class HagheMagharMap: EntityTypeConfiguration<HagheMagharModel>
    {
        public HagheMagharMap()
        {
            ToTable("Tb_HagheMaghar");
            HasKey(p => p.Id);
            Property(p => p.OrderItemId);
            Property(p => p.HagheMagharPrice);
            Property(p => p.ShipmentHagheMaghr);
        }
    }
}
