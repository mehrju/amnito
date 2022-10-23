using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class AssignAgentAmountRuleMap : EntityTypeConfiguration<AssignAgentAmountRuleModel>
    {
        public AssignAgentAmountRuleMap()
        {
            ToTable("Tb_CustomerAgentAssignmentRule");
            Property(p => p.CustomerId);
            Property(p => p.AgentAmountRuleId);
            Property(p => p.AssignmentDate);
            Property(p => p.IsActive);
            Property(p => p.AssignmentCustomerId);
            Property(p => p.DeActiveDate);
            Property(p => p.DeActiveCustomerId);
        }
    }
}
