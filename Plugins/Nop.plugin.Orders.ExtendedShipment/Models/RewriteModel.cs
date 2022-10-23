using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class RewriteModel
    {
        public int Id { get; set; }
        public string OldPath { get; set; }
        public string NewPath { get; set; }
        public bool IsActive { get; set; }
    }
}
