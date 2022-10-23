using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
   public class Tbl_Address_LatLong : BaseEntity
    {
        public int AddressId { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}
