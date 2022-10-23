using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
   public class Tbl_ContractItemDetail : BaseEntity
    {
        public int ContractItemId_fk { get; set; }
        public string ContractItemDetailesName { get; set; }
        public int? ContractItemDetailesCost { get; set; }
        public int? ContractItemDetailesPrice { get; set; }
        public int? ContractItemPercent { get; set; }
        public int? ContractItemNumber { get; set; }
        public int? DeActiveCustomer { get; set; }
        public DateTime? DeActiveDate { get; set; }
    }
}
