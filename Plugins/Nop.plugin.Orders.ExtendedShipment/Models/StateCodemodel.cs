using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class StateCodemodel:BaseEntity
    {
        public int stateId { get; set; }
        public string StateCode { get; set; }
        public string SenderStateCode { get; set; }
    }
}
