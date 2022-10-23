using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class SubMarketModel : BaseEntity
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public string UrlIdentifier { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefulte { get; set; }
    }
}
