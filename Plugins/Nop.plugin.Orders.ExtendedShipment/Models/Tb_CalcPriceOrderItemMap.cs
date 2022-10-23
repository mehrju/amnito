using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tb_CalcPriceOrderItemMap : EntityTypeConfiguration<Tb_CalcPriceOrderItem>
    {
        public Tb_CalcPriceOrderItemMap()
        {
            ToTable("Tb_CalcPriceOrderItem");
            HasKey(m => m.Id);
            Property(m => m.IncomePrice);
            Property(m => m.EngPrice);
            Property(m => m.AttrPrice);
            Property(m => m.OrderItemId);
            Property(m => m.TashimValue).IsOptional();
            Property(m => m.PricingPolicyId).IsOptional();

        }
    }
}
