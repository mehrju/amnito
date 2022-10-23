using Nop.Core;
using Nop.Plugin.Orders.MultiShipping.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Domain
{
    public class Tbl_ShipmentPackingRequest : BaseEntity
    {
        public int ShipmentId { get; set; }
        public string KartonSizeItemName { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsSmsSent { get; set; }
        [Required]
        public string CustomerPhone { get; set; }
        public ShipmentPackingRequestStatus Status { get; set; }
    }
}
