using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ChargeWalletHistoryMap:EntityTypeConfiguration<ChargeWalletHistoryModel>
    {
        public ChargeWalletHistoryMap() {
            ToTable("Tb_ChargeWalletHistory");
            HasKey(p => p.Id);
            Property(p => p.orderId);
            Property(p => p.orderItemId);
            Property(p=> p.shipmentId);
            Property(p=> p.rewaridPointHistoryId);
            Property(p=> p.AgentAmountRuleId);
            Property(p=> p.ChargeWalletTypeId);
            Property(p=> p.CustomerId);
            Property(p => p.Description);

        }
    }
}
