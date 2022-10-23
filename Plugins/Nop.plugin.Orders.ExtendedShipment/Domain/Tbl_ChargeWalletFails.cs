using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_ChargeWalletFails : BaseEntity
    {
        public string Message { get; set; }
        public string Data { get; set; }
        public string Exception { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
