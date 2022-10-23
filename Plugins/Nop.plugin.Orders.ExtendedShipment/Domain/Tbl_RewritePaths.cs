using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_RewritePaths : BaseEntity
    {
        public string OldPath { get; set; }
        public string NewPath { get; set; }
        public bool IsActive { get; set; }
    }
}
