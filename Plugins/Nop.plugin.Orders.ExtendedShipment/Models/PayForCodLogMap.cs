using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class PayForCodLogMap: EntityTypeConfiguration<PayForCodLogModel>
    {
        public PayForCodLogMap()
        {
            ToTable("PayForCodLog");
            HasKey(p => p.Id);
            Property(p => p.RewardPointHistoryId);
            Property(p => p.ShipmentId);
        }
    }
}
