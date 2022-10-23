using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
   public class Tbl_ContractItems : BaseEntity
    {
        public int ContractId_fk { get; set; }
        public int ContractItemsTypeId { get; set; }
        public int? ContractorCustomerId { get; set; }
        public int? DeActiveCustomer { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime? DeActiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
