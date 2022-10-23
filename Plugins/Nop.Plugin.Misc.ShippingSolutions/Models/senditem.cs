using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models
{
    public class senditem
    {
        public Listitem[] Listitem { get; set; }
        

    }
    public class Listitem
    {
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
        public float Percent { get; set; }
        public double Mablagh { get; set; }
        public double Tashim { get; set; }

        public float PercentTashim { get; set; }

    }
}
