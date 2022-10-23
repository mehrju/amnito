using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class OrderItemAttributeValueMap:EntityTypeConfiguration<OrderItemAttributeValueModel>
    {
        public OrderItemAttributeValueMap()
        {
            ToTable("Tb_OrderItemAttributeValue");
            Property(p => p.Id);
            Property(p => p.OrderItemId);
            Property(p => p.PropertyAttrId);
            Property(p => p.PropertyAttrName);
            Property(p => p.PropertyAttrValueCost);
            Property(p => p.PropertyAttrValueId);
            Property(p => p.PropertyAttrValueName);
            Property(p => p.PropertyAttrValuePrice);
            Property(p => p.PropertyAttrValueText);
            Property(p => p.PropertyAttrValueWeight);
        }
    }
}
