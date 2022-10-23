using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CustomOrder : Order
    {
        public int orderId { get; set; }
        public DateTime? CoordinationDate { get; set; }
        public string Desc { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public bool IsFirstORder { get; set; }
        public bool NeedPrinter { get; set; }
        public bool NeedCarton { get; set; }
        public bool IsUbaar { get; set; }
        public bool IsInternalForForeign { get; set; }
        public string CartonSizeName { get; set; }
        public CoardinationStatisticModel CoardinationStatistic { get; set; }
    }
}
