using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class AgentAmountRuleMap : EntityTypeConfiguration<AgentAmountRuleModel>
    {
        public AgentAmountRuleMap()
        {
            ToTable("Tb_AgentAmountRule");
            Property(p => p.CreateCustomerId);
            Property(p => p.CreateDate);
            Property(p => p.DeleteCustomerId);
            Property(p => p.DeletedDate);
            Property(p => p.Name);
            Property(p => p.Price);
            Property(p => p.ProductAttributeId);
            Property(p => p.ProductAttributeValueId);
            Property(p => p.ProductId);
            Property(p => p.MinCount);
            Property(p => p.MaxCount);
        }
    }
}
