using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_TrackingForeignOrder : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public int ShipingId { get; set; }
        
        [ScaffoldColumn(false)]
        public int Status { get; set; }
        [ScaffoldColumn(false)]
        public string Description { get; set; }
        [ScaffoldColumn(false)]
        public string IP { get; set; }
        [ScaffoldColumn(false)]
        public int Mablagh { get; set; }
        [ScaffoldColumn(false)]
        public bool IsPay { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
    }
}
