using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ChargeWalletHistoryModel : BaseEntity
    {
        public int orderId { get; set; }
        public int? orderItemId { get; set; }
        public int? shipmentId { get; set; }
        public int? Point { get; set; }
        public int rewaridPointHistoryId { get; set; }
        public int? AgentAmountRuleId { get; set; }
        public int? ChargeWalletTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
