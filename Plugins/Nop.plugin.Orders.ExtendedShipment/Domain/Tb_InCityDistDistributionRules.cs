using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tb_InCityDistDistributionRules : BaseEntity
    {
        public string SizeName { get; set; }
        public int TehranTotalPrice { get; set; }
        public int OtherCItyTotalPrice { get; set; }
        public int TehranAgentSharePrice { get; set; }
        public int OtherCityAgentSharePrice { get; set; }
        public int TehranOfficeSharePrice { get; set; }
        public int otherCitySharePrice { get; set; }
       
    }
}
