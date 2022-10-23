using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CountryCodeModel:BaseEntity
    {
        public int CountryId { get; set; }
        public int? printCountryCode { get; set; }
        public string CountryCode { get; set; }
    }
}
