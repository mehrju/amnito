using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_RelationOstanCityVijePost : BaseEntity
    {
        public int IdCountryRegion { get; set; }
        public int IdCountryDes { get; set; }
        public int? IdCity { get; set; }
        public string Name { get; set; }

    }
}
