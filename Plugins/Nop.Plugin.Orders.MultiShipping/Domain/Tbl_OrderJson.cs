using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Domain
{
    public class Tbl_OrderJson : BaseEntity
    {
        public int OrderId { get; set; }
        public string JsonData { get; set; }
        public bool IsWebApi { get; set; }
    }
}
