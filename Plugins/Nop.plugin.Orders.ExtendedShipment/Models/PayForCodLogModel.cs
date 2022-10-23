using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class PayForCodLogModel:BaseEntity
    {
        public int ShipmentId { get; set; }
        public int RewardPointHistoryId { get; set; }

    }
}
