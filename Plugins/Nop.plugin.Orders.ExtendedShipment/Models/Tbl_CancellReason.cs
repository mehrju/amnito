using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_CancellReason : BaseEntity
    {
        [ScaffoldColumn(false)]
        public string Description { get; set; }
        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }
    }
}
