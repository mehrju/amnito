using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_CityTarof : BaseEntity
    {
        public int _id { get; set; }
        public int idostan { get; set; }
        public string title { get; set; }
    }
}
