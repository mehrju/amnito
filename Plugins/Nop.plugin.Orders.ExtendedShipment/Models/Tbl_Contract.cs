using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
   public class Tbl_Contract : BaseEntity
    {
        public int CustomerId { get; set; }
        public int RegisterContract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? DeActtivadate { get; set; }
        public bool IsActive { get; set; }
    }
}
