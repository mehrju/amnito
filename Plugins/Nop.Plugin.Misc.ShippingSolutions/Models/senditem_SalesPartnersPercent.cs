using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models
{
    public class senditem_SalesPartnersPercent
    {
        public Listitem_SalesPartnersPercent[] Listitem_SalesPartnersPercent { get; set; }
        

    }
    public class Listitem_SalesPartnersPercent
    {
        public int id { get; set; }
        public int ofday { get; set; }
        public int upday { get; set; }
        public string name { get; set; }
        public float percent { get; set; }


    }
}
