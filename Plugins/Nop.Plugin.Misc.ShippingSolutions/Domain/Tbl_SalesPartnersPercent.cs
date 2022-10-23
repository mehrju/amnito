using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_SalesPartnersPercent : BaseEntity
    {
        [ScaffoldColumn(false)]
        public String Name { get; set; }
        [ScaffoldColumn(false)]
        public int OfDay { get; set; }
        [ScaffoldColumn(false)]
        public int UpDay { get; set; }
        [ScaffoldColumn(false)]
        public float _Percent { get; set; }
        
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }
    }
}
