using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_CollectorOptimeUser : BaseEntity
    {
        public int? CollectorCustomerId { get; set; }
        public int CountryId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
