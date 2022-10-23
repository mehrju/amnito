using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CancelOrderCostInfo
    {
        public string Type { get; set; }
        public int EnteredValue { get; set; }
        public int CalculatedTax
        {
            get
            {
                return EnteredValue * 9 / 100;
            }
        }
        public int AttrId { get; set; }
    }
}
