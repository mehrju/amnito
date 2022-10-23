using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_CancelReason_Order : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int  id_Description { get; set; }
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        public string RefoundItem { get; set; }
    }
}
